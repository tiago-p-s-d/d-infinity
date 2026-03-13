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

  login(dados: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, dados).pipe(
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
          id: decoded.sub,
          name: decoded.name,
          email: decoded.email
        });
      } catch (error) {
        this.userSubject.next(null);
      }
    }
  }

  getUser() {
    return this.userSubject.value;
  }

  register(dados: any) {
    return this.http.post(`${this.apiUrl}/register`, dados);
  }
}
