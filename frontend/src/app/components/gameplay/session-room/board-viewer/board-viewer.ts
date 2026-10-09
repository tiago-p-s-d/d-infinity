import { Component, input, output, signal, inject, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CampaignMap } from '../../../../interfaces/map.model';
import { GameObjects } from './game-objects/game-objects';
import { MapTokenService } from '../../../../services/gameplay/token/map-token-service';
import { MapService } from '../../../../services/gameplay/maps/map-service';

@Component({
  selector: 'app-board-viewer',
  standalone: true,
  imports: [CommonModule, FormsModule, GameObjects],
  templateUrl: './board-viewer.html',
  styleUrl: './board-viewer.scss',
})
export class BoardViewer {
  // Inputs
  campaignId = input<number>(0);
  currentMapUrl = input<string>('');
  currentMapId = input<number>(0);
  currentUserId = input<number>(0);
  currentCharacterSheetId = input<number>(0);
  isDm = input<boolean>(false);
  zoom = input<number>(1.0);
  gridCellSize = input<number>(70);

  // Outputs
  mapChanged = output<string>();
  mapSelected = output<CampaignMap>();
  zoomChanged = output<number>();
  tokenMovedSignalR = output<{ tokenId: number; coordX: number; coordY: number }>();
  gridCellSizeChanged = output<number>();

  // Injected services
  public tokenService = inject(MapTokenService);
  private mapService = inject(MapService);

  // Local reactive grid size state initialized from input
  activeGridCellSize = signal<number>(70);

  // Modals & State signals
  availableMaps = signal<CampaignMap[]>([]);
  loadingMaps = signal<boolean>(false);
  showMapModal = signal<boolean>(false);
  showTokenModal = signal<boolean>(false);

  newMapInput = '';
  newTokenName = '';
  newTokenImage = '';

  constructor() {
    // Synchronize active grid size whenever the input signal updates
    effect(() => {
      const incomingSize = this.gridCellSize();
      if (incomingSize > 0) {
        this.activeGridCellSize.set(incomingSize);
      }
    });

    // Load map tokens on active map change
    effect(() => {
      const mapId = this.currentMapId();
      if (mapId > 0) {
        this.tokenService.loadTokensByMap(mapId).subscribe({
          error: (err) => console.error('Error loading tokens for map:', err),
        });
      }
    });
  }

  canCreateToken = computed(() => {
    const hasMap = this.currentMapId() > 0 || Boolean(this.currentMapUrl());
    const isDmUser = this.isDm();
    const userId = this.currentUserId();

    if (!hasMap) return false;

    // DMs can always create tokens
    if (isDmUser) return true;

    // Prevent premature blocking before auth user ID loads
    if (userId <= 0) return false;

    // Standard player check: only allow creation if player does not already own a token on this map
    const alreadyHasToken = this.tokenService.tokens().some((t) => t.ownerUserId === userId);
    return !alreadyHasToken;
  });

  // Grid adjustment methods (DM only)
  decreaseGridSize(): void {
    if (!this.isDm()) return;
    const newSize = Math.max(20, this.activeGridCellSize() - 5);
    this.activeGridCellSize.set(newSize);
    this.gridCellSizeChanged.emit(newSize);
  }

  increaseGridSize(): void {
    if (!this.isDm()) return;
    const newSize = Math.min(200, this.activeGridCellSize() + 5);
    this.activeGridCellSize.set(newSize);
    this.gridCellSizeChanged.emit(newSize);
  }

  onFileSelected(event: Event): void {
    const inputEl = event.target as HTMLInputElement;
    if (inputEl.files && inputEl.files[0]) {
      const file = inputEl.files[0];
      const reader = new FileReader();
      reader.onload = () => {
        this.newTokenImage = reader.result as string;
      };
      reader.readAsDataURL(file);
    }
  }

  createToken(): void {
    const mapId = this.currentMapId();
    const userId = this.currentUserId();
    const sheetId = this.currentCharacterSheetId();

    if (!this.newTokenName.trim() || !this.newTokenImage.trim()) return;
    if (mapId <= 0) {
      alert('Cannot create token: No active map selected.');
      return;
    }

    this.tokenService.createToken({
      name: this.newTokenName.trim(),
      tokenImage: this.newTokenImage.trim(),
      mapId: mapId,
      characterSheetId: sheetId > 0 ? sheetId : null,
      ownerUserId: userId > 0 ? userId : null,
      coordX: 50,
      coordY: 50,
      size: 1.0,
    } as any).subscribe({
      next: () => {
        this.newTokenName = '';
        this.newTokenImage = '';
        this.showTokenModal.set(false);
      },
      error: (err) => {
        const errorDetail = err.error?.message || err.message || 'Unknown error';
        console.error('Error creating token:', errorDetail, err);
        alert(`Failed to create token: ${errorDetail}`);
      },
    });
  }

  onTokenMoved(event: { tokenId: number; coordX: number; coordY: number }): void {
    this.tokenService.updateTokenPositionInMemory(event.tokenId, event.coordX, event.coordY);
    this.tokenMovedSignalR.emit(event);
  }

  openMapSelector(): void {
    if (!this.isDm()) return;
    this.showMapModal.set(true);
    this.fetchSavedMaps();
  }

  fetchSavedMaps(): void {
    this.loadingMaps.set(true);
    this.mapService.getMaps().subscribe({
      next: (maps) => {
        this.availableMaps.set(maps);
        this.loadingMaps.set(false);
      },
      error: (err) => {
        console.error('Error fetching maps:', err);
        this.loadingMaps.set(false);
      },
    });
  }

  selectSavedMap(map: CampaignMap): void {
    const cId = this.campaignId();

    // 1. Persist active map to backend database for current campaign
    if (cId > 0 && map.id > 0) {
      this.mapService.setActiveMap(cId, map.id).subscribe({
        next: () => console.log(`Map ${map.id} successfully saved as active for campaign ${cId}`),
        error: (err) => console.error('Error saving active map to database:', err)
      });
    }

    // 2. Synchronize active map grid cell size if configured on the map record
    if (map.gridCellSize && map.gridCellSize > 0) {
      this.activeGridCellSize.set(map.gridCellSize);
      this.gridCellSizeChanged.emit(map.gridCellSize);
    }

    // 3. Emit map changes to local room and SignalR consumers
    this.mapChanged.emit(map.mapImage);
    this.mapSelected.emit(map);
    this.showMapModal.set(false);
  }

  applyCustomUrl(): void {
    if (this.newMapInput.trim()) {
      this.mapChanged.emit(this.newMapInput.trim());
      this.newMapInput = '';
      this.showMapModal.set(false);
    }
  }

  zoomIn(): void {
    const nextZoom = Math.min(Number((this.zoom() + 0.1).toFixed(1)), 3.0);
    this.zoomChanged.emit(nextZoom);
  }

  zoomOut(): void {
    const nextZoom = Math.max(Number((this.zoom() - 0.1).toFixed(1)), 0.3);
    this.zoomChanged.emit(nextZoom);
  }

  resetZoom(): void {
    this.zoomChanged.emit(1.0);
  }
}