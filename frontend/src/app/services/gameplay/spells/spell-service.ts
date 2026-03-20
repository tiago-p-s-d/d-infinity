import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SpellService {
  private apiUrl = `${environment.apiUrl}/spell`;

  constructor(private http: HttpClient) {}

  private getHeaders() {
    return new HttpHeaders({ 'Authorization': `Bearer ${localStorage.getItem('token')}` });
  }

  getSpells(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  create(spell: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, spell, { headers: this.getHeaders() });
  }

  update(id: number, spell: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, spell, { headers: this.getHeaders() });
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
