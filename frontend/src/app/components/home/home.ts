import { Component, OnInit } from '@angular/core';
import { Sidebar } from '../layout/sidebar/sidebar';
import { QuickTutorial } from '../dashboard/quick-tutorial/quick-tutorial';
import { StatsCards } from '../dashboard/stats-cards/stats-cards';
import { Auth } from '../../services/auth/auth';
@Component({
  selector: 'app-home',
  imports: [Sidebar, QuickTutorial, StatsCards],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home implements OnInit{
  user: any;
  constructor(private auth: Auth) {}
ngOnInit() {
    this.auth.user$.subscribe(userData => {
      this.user = userData;
    });
  }
}