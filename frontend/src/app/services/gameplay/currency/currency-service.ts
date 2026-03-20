import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CurrencyService {
  private apiUrl = `${environment.apiUrl}/currency`;

  constructor(private http: HttpClient) {}


  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }


  getCurrencies(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, { headers: this.getHeaders() });
  }


  getCurrencyById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }


  create(currency: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, currency, { headers: this.getHeaders() });
  }


  update(id: number, currency: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, currency, { headers: this.getHeaders() });
  }


  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
