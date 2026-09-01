// board-viewer.ts
import { Component, input, output, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CampaignMap } from '../../../../interfaces/map.model';

@Component({
  selector: 'app-board-viewer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './board-viewer.html',
  styleUrl: './board-viewer.scss',
})
export class BoardViewer {
  currentMapUrl = input<string>('');
  isDm = input<boolean>(false);
  zoom = input<number>(1.0);

  mapChanged = output<string>();
  zoomChanged = output<number>();

  private http = inject(HttpClient);

  availableMaps = signal<CampaignMap[]>([]);
  loadingMaps = signal<boolean>(false);
  showMapModal = signal<boolean>(false);
  newMapInput = '';

  openMapSelector(): void {
    if (!this.isDm()) return;
    this.showMapModal.set(true);
    this.fetchSavedMaps();
  }

  fetchSavedMaps(): void {
    this.loadingMaps.set(true);
    this.http.get<CampaignMap[]>('http://localhost:5000/api/Maps').subscribe({
      next: (maps) => {
        this.availableMaps.set(maps);
        this.loadingMaps.set(false);
      },
      error: (err) => {
        console.error('Erro ao buscar mapas:', err);
        this.loadingMaps.set(false);
      }
    });
  }

  selectSavedMap(map: CampaignMap): void {
    this.mapChanged.emit(map.mapImage);
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