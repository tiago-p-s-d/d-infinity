import { Component, OnInit, OnDestroy, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { BoardViewer } from './board-viewer/board-viewer';
import { Chat } from './chat/chat';
import { ChatService } from '../../../services/gameplay/chat/chat-service';
import { Auth } from '../../../services/auth/auth';
import { SheetService } from '../../../services/gameplay/character-sheet/sheet-service';
import { CampaignMap } from '../../../interfaces/map.model';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-session-room',
  standalone: true,
  imports: [CommonModule, BoardViewer, Chat],
  templateUrl: './session-room.html',
  styleUrl: './session-room.scss',
})
export class SessionRoom implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  public chatService = inject(ChatService);
  private auth = inject(Auth);
  private sheetService = inject(SheetService);

  campaignId = signal<number>(0);
  currentMapUrl = signal<string>('');
  currentMapId = signal<number>(0);
  currentZoom = signal<number>(1.0);
  //modificado
  currentGridCellSize = signal<number>(70);
  isDm = signal<boolean>(false);

  currentUserId = signal<number>(0);
  currentCharacterSheetId = signal<number>(0);

  private userSub?: Subscription;

  constructor() {
    effect(() => {
      const state = this.chatService.mapState();
      //modificado
      if (state && state.campaignId === this.campaignId() && state.mapUrl) {
        //modificado
        this.currentMapId.set(state.mapId);
        this.currentMapUrl.set(state.mapUrl);
        this.currentZoom.set(state.zoom || 1.0);
        //modificado
        if (state.gridCellSize && state.gridCellSize > 0) {
          //modificado
          this.currentGridCellSize.set(state.gridCellSize);
        //modificado
        }
      }
    });
  }

  ngOnInit(): void {
    // 1. Obtém o usuário logado
    this.userSub = this.auth.user$.subscribe((user) => {
      if (user?.id) {
        const userId = Number(user.id);
        this.currentUserId.set(userId);
        this.loadUserSheet(userId);
      }
    });

    // 2. Extrai o campaignId da rota e carrega a campanha (inclusive no F5)
    const routeId = this.route.snapshot.paramMap.get('id');
    if (routeId) {
      const id = Number(routeId);
      this.campaignId.set(id);
      //modificado
      this.chatService.startConnection(id);
      this.loadCampaignData(id);
    }
  }

  ngOnDestroy(): void {
    this.userSub?.unsubscribe();
    //modificado
    const cId = this.campaignId();
    //modificado
    if (cId > 0) {
      //modificado
      this.chatService.stopConnection(cId);
    //modificado
    }
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });
  }

  private loadCampaignData(id: number): void {
    this.http
      .get<any>(`${environment.apiUrl}/Campaign/${id}`, { headers: this.getAuthHeaders() })
      .subscribe({
        next: (res) => {
          this.isDm.set(Boolean(res.isDm));

          // Restaura o mapa ativo salvo no banco de dados (persistência pós-F5)
          if (res.currentMap) {
            this.currentMapId.set(res.currentMap.id);
            this.currentMapUrl.set(res.currentMap.mapImage);
            //modificado
            if (res.currentMap.gridCellSize) {
              //modificado
              this.currentGridCellSize.set(res.currentMap.gridCellSize);
            //modificado
            }
          }
        },
        error: (err) => console.error('Error loading campaign data:', err),
      });
  }

  private loadUserSheet(userId: number): void {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    const cId = this.campaignId();

    this.http
      .get<any[]>(`${environment.apiUrl}/character-sheet`, { headers })
      .subscribe({
        next: (sheets) => {
          console.log('DEBUG sheets returned for user:', sheets);

          // Busca ficha da campanha atual ou primeira ficha do jogador
          const mySheet = sheets.find(
            (s) => (s.playerId === userId || s.userId === userId) &&
              (!s.campaignId || s.campaignId === cId)
          ) || sheets.find((s) => s.playerId === userId || s.userId === userId);

          if (mySheet?.id) {
            console.log('DEBUG found sheet id:', mySheet.id);
            this.currentCharacterSheetId.set(mySheet.id);
          } else {
            console.warn('DEBUG: No character sheet matched for user', userId);
          }
        },
        error: (err) => console.error('Error loading user character sheet:', err),
      });
  }

  onMapSelected(map: CampaignMap): void {
    this.currentMapId.set(map.id);
    this.currentMapUrl.set(map.mapImage);
    //modificado
    const gridSize = (map as any).gridCellSize || this.currentGridCellSize();
    //modificado
    this.currentGridCellSize.set(gridSize);
    //modificado
    this.chatService.updateMapState(this.campaignId(), map.id, map.mapImage, this.currentZoom(), gridSize);
  }

  onMapChanged(newMapUrl: string): void {
    if (!this.isDm()) return;
    this.currentMapUrl.set(newMapUrl);
    //modificado
    this.chatService.updateMapState(this.campaignId(), this.currentMapId(), newMapUrl, this.currentZoom(), this.currentGridCellSize());
  }

  onZoomChanged(newZoom: number): void {
    if (!this.isDm()) return;
    this.currentZoom.set(newZoom);
    //modificado
    this.chatService.updateMapState(this.campaignId(), this.currentMapId(), this.currentMapUrl(), newZoom, this.currentGridCellSize());
  }

  //modificado
  onGridCellSizeChanged(newGridSize: number): void {
    //modificado
    if (!this.isDm()) return;
    //modificado
    this.currentGridCellSize.set(newGridSize);
    //modificado
    this.chatService.updateMapState(this.campaignId(), this.currentMapId(), this.currentMapUrl(), this.currentZoom(), newGridSize);
  //modificado
  }

  onTokenMoved(payload: { tokenId: number; coordX: number; coordY: number }): void {
    this.chatService.moveToken(this.campaignId(), payload);
  }
}