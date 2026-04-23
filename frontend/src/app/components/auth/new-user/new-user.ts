import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Auth } from '../../../services/auth/auth';

@Component({
  selector: 'app-new-user',
  standalone: true,
  imports: [ReactiveFormsModule, FormsModule, RouterModule, CommonModule],
  templateUrl: './new-user.html',
  styleUrl: './new-user.scss',
})
export class NewUser {
  userForm: FormGroup;
  isVerifying = false;
  isLoading = false;
  verificationCode = '';

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

  onSendCode() {
  if (this.userForm.invalid) return;

  // Change screen immediately to avoid UI hanging
  this.isVerifying = true; 
  const { email } = this.userForm.value;

  console.log('Sending request for:', email);

  this.auth.sendCode(email).subscribe({
    next: (res) => {
      console.log('Server acknowledged:', res);
      // Even if it takes time, the user is already on the code screen
    },
    error: (err) => {
      console.error('Request failed:', err);
      // Optional: if it fails miserably, go back or alert
      alert('Fill in the code once you receive it in your inbox.');
    }
  });
}

  onVerifyAndRegister() {
    if (this.verificationCode.length < 6) return;

    this.isLoading = true;
    const { email } = this.userForm.value;

    console.log('Verifying code...');
    this.auth.verifyCode(email, this.verificationCode).subscribe({
      next: () => {
        console.log('Code verified! Proceeding to registration...');
        
        this.auth.register(this.userForm.value).subscribe({
          next: () => {
            this.isLoading = false;
            console.log('Registration successful!');
            alert('Welcome to D-Infinity Forge!');
            this.router.navigate(['/login']);
          },
          error: (err) => {
            this.isLoading = false;
            console.error('Registration failed:', err);
            alert('Registration error: ' + (err.error?.message || 'Internal server error.'));
          }
        });
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Verification failed:', err);
        alert('Invalid or expired code. Please check your inbox.');
        this.verificationCode = ''; 
      }
    });
  }
}