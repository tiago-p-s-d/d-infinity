import { Component, Input, Output, EventEmitter, OnChanges, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CampaignService } from '../../../../services/campaign/campaign.service';

@Component({
  selector: 'app-sheet-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './sheet-edit-modal.html',
  styleUrl: './sheet-edit-modal.scss',
})
export class SheetEditModal implements OnChanges {
  @Input() sheet: any = null; // Objeto inicial (com campaignId e modelId)
  @Input() definitions: any[] = []; // O array do seu JSON: [{"name":"c1",...}]
  @Output() saved = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  sheetForm!: FormGroup;
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService
  ) { }

  ngOnChanges() {
    if (this.sheet && this.definitions) {
      this.buildForm();
    }
  }

  buildForm() {
    // Tenta ler valores existentes se for edição, senão usa vazio
    const currentValues = typeof this.sheet.Values === 'string' 
      ? JSON.parse(this.sheet.Values || '{}') 
      : (this.sheet.values || {});

    const controls: any = {
      characterName: [this.sheet.characterName || '', Validators.required]
    };

    // Monta os controles c1, c2, c3, c4 baseados nas definitions
    for (const field of this.definitions) {
      controls[field.name] = [currentValues[field.name] ?? ''];
    }
    
    this.sheetForm = this.fb.group(controls);
  }

  onSave() {
    if (this.sheetForm.invalid) return;

    this.loading.set(true);
    const { characterName, ...attributes } = this.sheetForm.value;

    // Montamos o payload para a rota /api/character-sheet
    const payload = {
      id: this.sheet.id || 0, // 0 se for novo
      characterName: characterName,
      modelId: this.sheet.modelId,
      campaignId: this.sheet.campaignId,
      values: JSON.stringify(attributes) // Os campos c1, c2... viram JSON
    };

    // Decisão de rota baseada na existência do ID
    const request = (this.sheet.id) 
      ? this.campaignService.updateSheet(this.sheet.id, payload) 
      : this.campaignService.createSheet(payload);

    request.subscribe({
      next: () => {
        this.loading.set(false);
        this.saved.emit();
      },
      error: (err: any) => {
        console.error('Error saving sheet:', err);
        this.loading.set(false);
      }
    });
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closed.emit();
    }
  }

  getFieldType(type: string): string {
    return type === 'number' ? 'number' : 'text';
  }
}