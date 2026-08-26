import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Header } from '../../layout/header/header';
import { CampaignService } from '../../../services/campaign/campaign.service';
import { SheetEditModal } from './sheet-edit-modal/sheet-edit-modal';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-join-campaign',
  imports: [CommonModule, ReactiveFormsModule, RouterModule, Header, SheetEditModal],
  templateUrl: './join-campaign.html',
  styleUrl: './join-campaign.scss',
})
export class JoinCampaign implements OnInit {
  inviteForm!: FormGroup;

  myCampaigns = signal<any[]>([]);
  error = signal<string | null>(null);
  loading = signal(false);
  joinSuccess = signal(false);

  modalOpen = false;
  modalSheet = signal<any>(null);
  modalDefinitions = signal<any[]>([]);

  constructor(
    private fb: FormBuilder,
    private campaignService: CampaignService
  ) { }

  ngOnInit() {
    this.inviteForm = this.fb.group({
      inviteCode: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(8)]]
    });

    this.loadMyCampaigns();
  }

  loadMyCampaigns() {
    this.campaignService.getMyCampaignsAsPlayer().subscribe({
      next: (data) => this.myCampaigns.set(data),
      error: (err) => console.error(err)
    });
  }

  onJoin() {
    if (this.inviteForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);
    this.joinSuccess.set(false);

    const code = this.inviteForm.value.inviteCode.trim();

    this.campaignService.joinCampaign(code).subscribe({
      next: () => {
        this.inviteForm.reset();
        this.joinSuccess.set(true);
        this.loadMyCampaigns();
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error || 'Invalid invite code.');
        this.loading.set(false);
      }
    });
  }

  openSheetModal(campaign: any) {
    if (!campaign.campaignId || !campaign.systemId) return;

    this.loading.set(true);

    forkJoin({
      sheet: this.campaignService.getSheetByCampaign(campaign.campaignId),
      model: this.campaignService.getSheetModelBySystem(campaign.systemId)
    }).subscribe({
      next: (res) => {
        const defs = res.model?.definitions || res.model?.Definitions || [];
        this.modalDefinitions.set(defs);

        if (res.sheet) {
          // Normaliza values para garantir que seja um objeto acessível
          let parsedValues = res.sheet.values ?? res.sheet.Values ?? {};
          if (typeof parsedValues === 'string') {
            try {
              parsedValues = JSON.parse(parsedValues);
            } catch {
              parsedValues = {};
            }
          }

          this.modalSheet.set({
            ...res.sheet,
            values: parsedValues
          });
        } else {
          this.modalSheet.set({
            id: null,
            characterName: '',
            campaignId: campaign.campaignId,
            modelId: res.model?.id || res.model?.Id,
            values: {}
          });
        }

        this.modalOpen = true;
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading sheet data:', err);
        this.loading.set(false);
        alert('Could not load character data.');
      }
    });
  }

  closeModal() {
    this.modalOpen = false;
    this.modalSheet.set(null);
  }

  onSheetSaved() {
    this.closeModal();
    this.loadMyCampaigns();
  }
}