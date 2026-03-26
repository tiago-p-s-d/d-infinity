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
    if (this.userForm.valid) {
      this.isLoading = true;
      const email = this.userForm.value.email;

      this.auth.sendCode(email).subscribe({
        next: () => {
          this.isLoading = false;
          this.isVerifying = true;
          this.verificationCode = ''; // Limpa caso seja um re-envio
        },
        error: (err) => {
          this.isLoading = false;
          alert('Error: ' + (err.error?.message || 'Check your internet connection.'));
        }
      });
    }
  }

  onVerifyAndRegister() {
    if (this.verificationCode.length < 6) return;

    this.isLoading = true;
    const { email } = this.userForm.value;

    this.auth.verifyCode(email, this.verificationCode).subscribe({
      next: () => {
        // Código válido! Agora chamamos o registro final
        this.auth.register(this.userForm.value).subscribe({
          next: () => {
            this.isLoading = false;
            alert('Welcome to D-Infinity Forge!');
            this.router.navigate(['/login']);
          },
          error: (err) => {
            this.isLoading = false;
            alert('Registration error: ' + (err.error?.message || 'Check your database.'));
          }
        });
      },
      error: () => {
        this.isLoading = false;
        alert('Invalid or expired code. Please check your email.');
        this.verificationCode = ''; // Limpa para o usuário tentar de novo
      }
    });
  }
}