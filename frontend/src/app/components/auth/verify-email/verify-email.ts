import { Component, ElementRef, ViewChildren, QueryList, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';

import { Auth } from '../../../services/auth/auth';

@Component({
  selector: 'app-verify-email',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.scss',
})
export class VerifyEmail implements OnInit{
  @ViewChildren('codeInput') codeInputs!: QueryList<ElementRef>;
  
  email: string = ''; 
  code: string[] = ['', '', '', '', '', '']; 
  isLoading = false;
  errorMessage = '';
  isSuccess = false;

  constructor(
    private auth: Auth,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Get the email from the URL query params (e.g., /verify-email?email=test@test.com)
    // Your Registration component should navigate here passing this param.
    this.route.queryParams.subscribe(params => {
      this.email = params['email'];
      if (!this.email) {
        // If no email, redirect back to register
        this.router.navigate(['/new-user']);
      }
    });
  }

  // Handle keyup events for auto-focus navigation
  onKeyUp(event: KeyboardEvent, index: number) {
    const input = event.target as HTMLInputElement;
    const value = input.value;

    // Sanitize: Ensure only numbers are kept
    this.code[index] = value.replace(/[^0-9]/g, '').slice(0, 1);

    if (value && index < 5) {
      // Move to next input if value entered
      this.codeInputs.toArray()[index + 1].nativeElement.focus();
    } else if (event.key === 'Backspace' && index > 0) {
      // Move to previous input on backspace
      this.codeInputs.toArray()[index - 1].nativeElement.focus();
    }

    // If all inputs are filled, trigger verification
    if (this.code.every(digit => digit !== '')) {
      this.verify();
    }
  }

  // Handle pasting the full code (e.g., Ctrl+V "123456")
  onPaste(event: ClipboardEvent) {
    const pastedData = event.clipboardData?.getData('text');
    if (pastedData && /^[0-9]{6}$/.test(pastedData)) {
      event.preventDefault();
      // Distribute digits into the array
      this.code = pastedData.split('');
      // Focus the last input
      this.codeInputs.last.nativeElement.focus();
      this.verify();
    }
  }

  verify() {
    if (this.isLoading) return;
    
    this.isLoading = true;
    this.errorMessage = '';
    const finalCode = this.code.join('');

    this.auth.verifyCode(this.email, finalCode).subscribe({
      next: () => {
        this.isLoading = false;
        this.isSuccess = true;
        alert('Email verified successfully! You can now login.');
        // Navigate to login after a short delay
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Invalid or expired code.';
        // Clear inputs on error for retry
        this.code = ['', '', '', '', '', ''];
        this.codeInputs.first.nativeElement.focus();
      }
    });
  }
}
