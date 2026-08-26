import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, BehaviorSubject, Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

export interface ForgotPasswordRequest {
  email: string;
}

export interface VerifyResetCodeRequest {
  email: string;
  code: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}

export interface AuthResponse {
  token?: string;
  message?: string;
}

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private apiUrl = 'http://localhost:5000/api/auth';

  private userSubject = new BehaviorSubject<any>(null);
  public user$ = this.userSubject.asObservable();

  constructor(private http: HttpClient) {
    this.refreshUser();
  }

  register(userData: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, userData);
  }

  login(credentials: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((res) => {
        if (res.token) {
          localStorage.setItem('token', res.token);
          this.refreshUser();
        }
      })
    );
  }

  forgotPassword(data: ForgotPasswordRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/forgot-password`, data);
  }

  verifyResetCode(data: VerifyResetCodeRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/verify-reset-code`, data);
  }

  resetPassword(data: ResetPasswordRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/reset-password`, data);
  }

  logout(): void {
    localStorage.removeItem('token');
    this.userSubject.next(null);
  }

  getUser(): any {
    return this.userSubject.value;
  }

  sendVerificationCode(email: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/send-verification-code`, { email });
  }
  sendCode(email: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/send-code`, { email });
  }

  verifyCode(email: string, code: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/verify-code`, { email, code });
  }

  private refreshUser(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);

        // Supports standard ClaimTypes mapped by ASP.NET Core JWT
        this.userSubject.next({
          id: decoded.nameid || decoded.sub || decoded.id,
          name: decoded.unique_name || decoded.name,
          email: decoded.email
        });
      } catch (error) {
        localStorage.removeItem('token');
        this.userSubject.next(null);
      }
    }
  }
}