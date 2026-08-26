import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';


@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [ RouterModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  showCreateModal = false;

  constructor(private router: Router) {}

  handleCreation(type: string) {

  }
  onLogout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }
}
