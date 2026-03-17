import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

export interface SystemModel {
  id: number;
  name: string;
}

@Injectable({
  providedIn: 'root',
})
export class SystemService {
  private apiUrl = `${environment.apiUrl}/api/system`;

  constructor(private http: HttpClient) {}

  getSystems(): Observable<SystemModel[]> {
    const token = localStorage.getItem('token');
    
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<SystemModel[]>(this.apiUrl, { headers });
  }
}
