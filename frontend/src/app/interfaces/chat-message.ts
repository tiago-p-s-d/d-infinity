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