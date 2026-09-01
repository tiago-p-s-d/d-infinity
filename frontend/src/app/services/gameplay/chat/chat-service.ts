import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { ChatMessage } from '../../../interfaces/chat-message';

export interface MapState {
  campaignId: number;
  mapUrl: string;
  zoom: number;
}

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private apiUrl = 'http://localhost:5000/api/campaigns';
  private hubUrl = 'http://localhost:5000/hubs/chat';

  private hubConnection: signalR.HubConnection | null = null;
  public messages = signal<ChatMessage[]>([]);
  public isConnected = signal<boolean>(false);
  public mapState = signal<MapState>({ campaignId: 0, mapUrl: '', zoom: 1.0 });

  constructor(private http: HttpClient) {}

  public loadHistory(campaignId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/${campaignId}/chat`).pipe(
      tap((history) => this.messages.set(history))
    );
  }

  public sendMessage(campaignId: number, content: string): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(`${this.apiUrl}/${campaignId}/chat`, { content });
  }

  public updateMapState(campaignId: number, mapUrl: string, zoom: number): void {
    // 1. Atualização otimista imediata na tela do DM
    this.mapState.set({ campaignId, mapUrl, zoom });

    // 2. Disparo via WebSocket para sincronizar os outros jogadores
    if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
      this.hubConnection
        .invoke('UpdateMapState', campaignId, mapUrl, zoom)
        .catch((err) => console.error('Erro ao emitir estado do mapa via SignalR:', err));
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