import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Auth } from '../../../services/auth/auth';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss',
})
export class ForgotPassword {
  forgotPasswordForm: FormGroup;
  step: number = 1;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private auth: Auth,
    private router: Router
  ) {
    this.forgotPasswordForm = this.fb.group(
      {
        email: ['', [Validators.required, Validators.email]],
        code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
        newPassword: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );
  }

  private passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPassword = control.get('newPassword')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }

  onSendCode(): void {
    const emailControl = this.forgotPasswordForm.get('email');
    if (!emailControl || emailControl.invalid) {
      return;
    }

    this.isLoading = true;
    const email = emailControl.value;

    this.auth.forgotPassword({ email }).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        alert(res?.message || 'If registered, a recovery code has been sent.');
        this.step = 2;
      },
      error: (err: any) => {
        this.isLoading = false;
        alert('Request failed: ' + (err.error?.message || 'Something went wrong.'));
      },
    });
  }

  onResetPassword(): void {
    if (this.forgotPasswordForm.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, code, newPassword } = this.forgotPasswordForm.value;

    this.auth.resetPassword({ email, code, newPassword }).subscribe({
      next: () => {
        this.isLoading = false;
        alert('Password updated successfully! Please log in.');
        this.router.navigate(['/login']);
      },
      error: (err: any) => {
        this.isLoading = false;
        alert('Failed to reset password: ' + (err.error?.message || 'Invalid or expired code.'));
      },
    });
  }
}