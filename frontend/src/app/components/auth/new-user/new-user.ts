import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import { Auth } from '../../../services/auth/auth';

@Component({
  selector: 'app-new-user',
imports: [ReactiveFormsModule, RouterModule, CommonModule],
  templateUrl: './new-user.html',
  styleUrl: './new-user.scss',
})
export class NewUser {
userForm: FormGroup;

  constructor(
    private fb: FormBuilder, 
    private auth: Auth,
    private router: Router
  ) {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(3)]]
    });
  }

  onSubmit() {
    if (this.userForm.valid) {
      this.auth.register(this.userForm.value).subscribe({
        next: () => {
          alert('User created successfully!');
          this.router.navigate(['/login']);
        },
        error: (err) => alert('Error creating user: ' + err.error.message)
      });
    }
  }
}
