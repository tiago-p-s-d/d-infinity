import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http'; // Adicionado HttpHeaders
import { environment } from '../../../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ItemGroupService {
  
  private apiUrl = `${environment.apiUrl}/item/groups`;

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getGroups(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  createGroup(name: string): Observable<any> {
    return this.http.post<any>(this.apiUrl, { name }, { headers: this.getHeaders() });
  }
}