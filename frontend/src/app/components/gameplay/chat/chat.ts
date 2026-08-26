import { Component, Input, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { ChatService } from '../../../services/gameplay/chat/chat-service';
import { Auth } from '../../../services/auth/auth';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat implements OnInit, OnDestroy, AfterViewChecked {
  @Input() campaignId: number = 1;
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;

  messageContent = '';
  loading = false;
  currentUserId = signal<number>(0);

  private userSub?: Subscription;

  constructor(
    public chatService: ChatService,
    private auth: Auth,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const routeId = this.route.snapshot.paramMap.get('id');
    if (routeId) {
      this.campaignId = Number(routeId);
    }

    this.userSub = this.auth.user$.subscribe((user) => {
      if (user?.id) {
        this.currentUserId.set(Number(user.id));
      }
    });

    this.chatService.loadHistory(this.campaignId).subscribe();
    this.chatService.startConnection(this.campaignId);
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  ngOnDestroy(): void {
    this.userSub?.unsubscribe();
    this.chatService.stopConnection(this.campaignId);
  }

  isMyMessage(messageUserId: number | string): boolean {
    const myId = this.currentUserId();
    return myId > 0 && myId === Number(messageUserId);
  }

  onSend(): void {
    const content = this.messageContent.trim();
    if (!content || this.loading) return;

    this.loading = true;
    this.chatService.sendMessage(this.campaignId, content).subscribe({
      next: () => {
        this.messageContent = '';
        this.loading = false;
      },
      error: (err) => {
        console.error('Failed to send message:', err);
        this.loading = false;
      }
    });
  }

  private scrollToBottom(): void {
    try {
      if (this.scrollContainer) {
        this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
      }
    } catch {}
  }
}