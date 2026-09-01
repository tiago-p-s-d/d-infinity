// session-room.component.ts
import { Component, OnInit, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { BoardViewer } from './board-viewer/board-viewer';
import { Chat } from './chat/chat';
import { ChatService } from '../../../services/gameplay/chat/chat-service';

@Component({
  selector: 'app-session-room',
  standalone: true,
  imports: [CommonModule, BoardViewer, Chat],
  templateUrl: './session-room.html',
  styleUrl: './session-room.scss'
})
export class SessionRoom implements OnInit {
  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  public chatService = inject(ChatService);

  campaignId = signal<number>(1);
  currentMapUrl = signal<string>('');
  currentZoom = signal<number>(1.0);
  isDm = signal<boolean>(false);

  constructor() {
    effect(() => {
      const state = this.chatService.mapState();
      if (state && state.campaignId === this.campaignId() && state.mapUrl) {
        this.currentMapUrl.set(state.mapUrl);
        this.currentZoom.set(state.zoom || 1.0);
      }
    });
  }

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (routeId) {
      const id = Number(routeId);
      this.campaignId.set(id);
      this.loadCampaignRole(id);
    }
  }

  private loadCampaignRole(id: number): void {
    this.http.get<any>(`http://localhost:5000/api/Campaign/${id}`).subscribe({
      next: (campaign) => {
        this.isDm.set(campaign.isDm);
      }
    });
  }

  onMapChanged(newMapUrl: string): void {
    if (!this.isDm()) return;
    this.currentMapUrl.set(newMapUrl);
    this.chatService.updateMapState(this.campaignId(), newMapUrl, this.currentZoom());
  }

  onZoomChanged(newZoom: number): void {
    if (!this.isDm()) return;
    this.currentZoom.set(newZoom);
    this.chatService.updateMapState(this.campaignId(), this.currentMapUrl(), newZoom);
  }
}