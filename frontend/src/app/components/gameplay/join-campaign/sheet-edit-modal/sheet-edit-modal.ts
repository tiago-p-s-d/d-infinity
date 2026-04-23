import { Component, Input, Output, EventEmitter, OnChanges, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CampaignService } from '../../../../services/campaign/campaign.service';

@Component({
  selector: 'app-sheet-edit-modal',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './sheet-edit-modal.html',
  styleUrl: './sheet-edit-modal.scss',
})
export class SheetEditModal implements OnChanges {
  @Input() open = false;
  @Input() sheet: any = null;
  @Input() definitions: any[] = [];
  @Output() saved = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  sheetForm!: FormGroup;
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService
  ) { }

  ngOnChanges() {
    if (this.open && this.sheet && this.definitions) {
      this.buildForm();
    }
  }

  buildForm() {
    const controls: any = {};
    for (const field of this.definitions) {
      const savedValue = this.sheet?.data?.[field.name] ?? field.defaultValue ?? '';
      controls[field.name] = [savedValue];
    }
    this.sheetForm = this.fb.group(controls);
  }

  onSave() {
    if (!this.sheetForm || this.sheetForm.invalid) return;

    this.loading.set(true);

    const payload = {
      campaignId: this.sheet.campaignId,
      data: this.sheetForm.value
    };

    this.campaignService.updateSheet(this.sheet.id, payload).subscribe({
      next: () => {
        this.loading.set(false);
        this.saved.emit();
      },
      error: (err) => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  getFieldType(type: string): string {
    const map: Record<string, string> = {
      text: 'text',
      number: 'number',
      email: 'email',
      url: 'url',
    };
    return map[type] ?? 'text';
  }

  onBackdropClick(event: MouseEvent) {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closed.emit();
    }
  }
}