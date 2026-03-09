import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class Auth {
  private apiUrl = 'http://localhost:5000/api/auth';

  constructor(private http: HttpClient) {}

  login(dados: any) {
    return this.http.post<any>(`${this.apiUrl}/login`, dados).pipe(
      tap(res => {
        if (res.token) localStorage.setItem('token', res.token);
      })
    );
  }

  register(dados: any) {
    return this.http.post(`${this.apiUrl}/register`, dados);
  }
}
