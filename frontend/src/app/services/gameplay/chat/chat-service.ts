import { Injectable, signal, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { ChatMessage } from '../../../interfaces/chat-message';
import { MapTokenService } from '../token/map-token-service';

export interface MapState {
  campaignId: number;
  mapId: number; 
  mapUrl: string;
  zoom: number;
  gridCellSize?: number; 
}

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private apiUrl = 'http://localhost:5000/api/campaigns';
  private hubUrl = 'http://localhost:5000/hubs/chat';

  private mapTokenService = inject(MapTokenService);

  private hubConnection: signalR.HubConnection | null = null;
  public messages = signal<ChatMessage[]>([]);
  public isConnected = signal<boolean>(false);
  public mapState = signal<MapState>({ campaignId: 0, mapId: 0, mapUrl: '', zoom: 1.0, gridCellSize: 70 }); 

  constructor(private http: HttpClient) {}

  public loadHistory(campaignId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/${campaignId}/chat`).pipe(
      tap((history) => this.messages.set(history))
    );
  }

  public sendMessage(campaignId: number, content: string): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(`${this.apiUrl}/${campaignId}/chat`, { content });
  }

  public updateMapState(campaignId: number, mapId: number, mapUrl: string, zoom: number, gridCellSize: number = 70): void { 
    const safeMapId = Math.round(mapId || 0); 
    const safeGridSize = Math.round(gridCellSize || 70); 
    const safeZoom = Number(zoom || 1.0); 

    this.mapState.set({ campaignId, mapId: safeMapId, mapUrl, zoom: safeZoom, gridCellSize: safeGridSize }); 

    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection
        .invoke('UpdateMapState', campaignId, safeMapId, mapUrl, safeZoom, safeGridSize) 
        .catch((err) => console.error('Error emitting map state via SignalR:', err));
    }
  }

  public moveToken(campaignId: number, payload: { tokenId: number; coordX: number; coordY: number }): void {
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection
        .invoke('MoveToken', campaignId, payload)
        .catch((err) => console.error('Error invoking MoveToken on SignalR Hub:', err));
    }
  }

  public startConnection(campaignId: number): void {
    if (
      this.hubConnection &&
      (this.hubConnection.state === signalR.HubConnectionState.Connected ||
        this.hubConnection.state === signalR.HubConnectionState.Connecting)
    ) {
      return;
    }

    const token = localStorage.getItem('token') || '';

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      this.messages.update((prev) => [...prev, message]);
    });

    this.hubConnection.on('ReceiveMapState', (state: MapState) => {
      this.mapState.set(state); 
    });

    this.hubConnection.on(
      'ReceiveTokenMoved',
      (data: { tokenId: number; coordX: number; coordY: number; movedByUserId: number }) => {
        this.mapTokenService.updateTokenPositionInMemory(data.tokenId, data.coordX, data.coordY);
      }
    );

    this.hubConnection.onreconnected(() => {
      this.isConnected.set(true);
      this.hubConnection?.invoke('JoinCampaignRoom', campaignId);
    });

    this.hubConnection.onclose(() => {
      this.isConnected.set(false);
    });

    this.hubConnection
      .start()
      .then(() => {
        this.isConnected.set(true);
        return this.hubConnection?.invoke('JoinCampaignRoom', campaignId);
      })
      .catch((err) => {
        this.isConnected.set(false);
        console.error('SignalR Connection Error:', err);
      });
  }

  public stopConnection(campaignId: number): void {
    if (this.hubConnection) {
      if (this.hubConnection.state === signalR.HubConnectionState.Connected) {
        this.hubConnection
          .invoke('LeaveCampaignRoom', campaignId)
          .catch(() => {})
          .finally(() => {
            this.hubConnection?.stop();
            this.isConnected.set(false);
          });
      } else {
        this.hubConnection.stop();
        this.isConnected.set(false);
      }
    }
  }
}