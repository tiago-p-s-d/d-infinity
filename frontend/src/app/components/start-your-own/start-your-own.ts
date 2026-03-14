import { Component, OnInit } from '@angular/core';
import { CampaignService } from '../../services/campaign/campaign.service';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth/auth';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';


@Component({
  selector: 'app-start-your-own',
  imports: [ReactiveFormsModule],
  templateUrl: './start-your-own.html',
  styleUrl: './start-your-own.scss',
})
export class StartYourOwn implements OnInit {
  campaignForm!: FormGroup;
  systems = ['D&D 5e', 'Pathfinder 2e', 'Custom'];

  constructor(
    private fb: FormBuilder,
    private auth: Auth,
    private campaignService: CampaignService,
    private router: Router
  ) { }

  ngOnInit() {
    this.auth.user$.subscribe(user => {
      this.campaignForm = this.fb.group({
        name: [`${user?.name || 'My'}'s Campaign`, [Validators.required]],
        about: ['', [Validators.maxLength(500)]],
        system: ['D&D 5e', [Validators.required]]
      });
    });
  }

  onSubmit() {
    if (this.campaignForm.valid) {
      this.campaignService.startYourOwn(this.campaignForm.value).subscribe({
        next: (res) => this.router.navigate(['/campaign', res.id]),
        error: (err) => console.error('Error creating campaign', err)
      });
    }
  }
}
