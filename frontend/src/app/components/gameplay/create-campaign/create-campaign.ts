import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SystemService } from '../../../services/gameplay/systems/system-service';
import { CampaignService } from '../../../services/campaign/campaign.service';
import { Header } from '../../layout/header/header';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';

@Component({
  selector: 'app-create-campaign',
  imports: [CommonModule, ReactiveFormsModule, Header, EditButton, DeleteButton],
  templateUrl: './create-campaign.html',
  styleUrl: './create-campaign.scss',
})
export class CreateCampaign implements OnInit {
  campaignForm!: FormGroup;
  systems = signal<any[]>([]);
  campaigns = signal<any[]>([]);
  isEditing = false;
  editingId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private systemService: SystemService,
    private campaignService: CampaignService
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadSystems();
    this.loadCampaigns();
  }

  initForm() {
    this.campaignForm = this.fb.group({
      campaignName: ['', [Validators.required]],
      systemId: [null, [Validators.required]],
      about: ['']
    });
  }

  loadSystems() {
    this.systemService.getSystems().subscribe({
      next: (data) => this.systems.set(data),
      error: (err) => console.error('Error loading systems', err)
    });
  }

  loadCampaigns() {
    this.campaignService.getCampaigns().subscribe({
      next: (data) => this.campaigns.set(data),
      error: (err) => console.error('Error loading campaigns', err)
    });
  }

  onCreate() {
    if (this.campaignForm.invalid) return;

    const payload = this.campaignForm.value;

    if (this.isEditing && this.editingId !== null) {
      this.campaignService.updateCampaign(this.editingId, payload).subscribe({
        next: () => { this.loadCampaigns(); this.resetForm(); },
        error: (err) => console.error('Error updating campaign', err)
      });
    } else {
      this.campaignService.createCampaign(payload).subscribe({
        next: () => { this.loadCampaigns(); this.resetForm(); },
        error: (err) => console.error('Error creating campaign', err)
      });
    }
  }

  onEdit(campaign: any) {
    this.isEditing = true;
    this.editingId = campaign.id;
    this.campaignForm.patchValue({
      campaignName: campaign.campaignName,
      systemId: campaign.systemId,
      about: campaign.about
    });
    document.getElementById('raceFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  onDelete(campaign: any) {
    if (!confirm(`Delete campaign "${campaign.campaignName}"?`)) return;

    this.campaignService.deleteCampaign(campaign.id).subscribe({
      next: () => this.loadCampaigns(),
      error: (err) => console.error('Error deleting campaign', err)
    });
  }

  resetForm() {
    this.campaignForm.reset({ campaignName: '', systemId: null, about: '' });
    this.isEditing = false;
    this.editingId = null;
  }

  copyInvite(code: string) {
    navigator.clipboard.writeText(code);
  }
}