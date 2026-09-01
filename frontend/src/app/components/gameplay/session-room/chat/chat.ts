import {
  Component,
  Input,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  signal,
  effect,
  inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ChatService } from '../../../../services/gameplay/chat/chat-service';
import { Auth } from '../../../../services/auth/auth';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat implements OnInit, OnDestroy {
  @Input({ required: true }) campaignId!: number;
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef<HTMLElement>;

  public chatService = inject(ChatService);
  private auth = inject(Auth);

  messageContent = '';
  currentUserId = signal<number>(0);
  chatMode = signal<'sidebar' | 'floating'>('sidebar');

  private userSub?: Subscription;

  constructor() {
    effect(() => {
      this.chatService.messages();
      requestAnimationFrame(() => this.scrollToBottom());
    });
  }

  ngOnInit(): void {
    this.userSub = this.auth.user$.subscribe((user) => {
      if (user?.id) {
        this.currentUserId.set(Number(user.id));
      }
    });

    this.chatService.loadHistory(this.campaignId).subscribe();
    this.chatService.startConnection(this.campaignId);
  }

  ngOnDestroy(): void {
    this.userSub?.unsubscribe();
    this.chatService.stopConnection(this.campaignId);
  }

  toggleChatMode(): void {
    this.chatMode.update((mode) => (mode === 'sidebar' ? 'floating' : 'sidebar'));
  }

  isMyMessage(messageUserId: number | string): boolean {
    const myId = this.currentUserId();
    return myId > 0 && myId === Number(messageUserId);
  }

  onSend(): void {
    const content = this.messageContent.trim();
    if (!content) return;

    this.messageContent = '';
    this.chatService.sendMessage(this.campaignId, content).subscribe({
      error: (err) => console.error('Erro ao enviar mensagem:', err)
    });
  }

  private scrollToBottom(): void {
    const container = this.scrollContainer?.nativeElement;
    if (container) {
      container.scrollTop = container.scrollHeight;
    }
  }
}