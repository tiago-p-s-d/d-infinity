import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import * as signalR from '@microsoft/signalr';

export interface ChatMessage {
  id: number;
  campaignId: number;
  userId: number;
  senderName: string;
  content: string;
  type: 'Text' | 'DiceRoll' | 'System';
  metadataJson?: string;
  sentAt: string;
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

  constructor(private http: HttpClient) {}

  public loadHistory(campaignId: number): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/${campaignId}/chat`).pipe(
      tap((history) => this.messages.set(history))
    );
  }

  public sendMessage(campaignId: number, content: string): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(`${this.apiUrl}/${campaignId}/chat`, { content });
  }

  public startConnection(campaignId: number): void {
    const token = localStorage.getItem('token') || '';

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.isConnected.set(true);
        this.hubConnection?.invoke('JoinCampaignRoom', campaignId);
      })
      .catch((err) => console.error('SignalR Connection Error: ', err));

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      this.messages.update((prev) => [...prev, message]);
    });
  }

  public stopConnection(campaignId: number): void {
    if (this.hubConnection) {
      this.hubConnection.invoke('LeaveCampaignRoom', campaignId).finally(() => {
        this.hubConnection?.stop();
        this.isConnected.set(false);
      });
    }
  }
}