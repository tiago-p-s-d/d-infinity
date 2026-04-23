import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, BehaviorSubject, Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

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

  sendCode(email: string): Observable<any> {
    // Keep this exact format since your Backend expects a raw JSON-quoted string
    return this.http.post(`${this.apiUrl}/send-code`, JSON.stringify(email), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  verifyCode(email: string, code: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/verify-code`, { email, code });
  }

  login(credentials: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(res => {
        if (res.token) {
          localStorage.setItem('token', res.token);
          this.refreshUser();
        }
      })
    );
  }

  private refreshUser() {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);
        this.userSubject.next({
          id: decoded.id, 
          name: decoded.name,
          email: decoded.email
        });
      } catch (error) {
        localStorage.removeItem('token');
        this.userSubject.next(null);
      }
    }
  }

  logout() {
    localStorage.removeItem('token');
    this.userSubject.next(null);
  }

  getUser() {
    return this.userSubject.value;
  }

  register(userData: any) {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }
}