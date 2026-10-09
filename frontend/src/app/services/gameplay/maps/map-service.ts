import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MapService {
  private apiUrl = `${environment.apiUrl}/Maps`;
  private campaignApiUrl = `${environment.apiUrl}/Campaign`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    });
  }

  getMaps(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  getMapById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  createMap(map: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, map, { headers: this.getHeaders() });
  }

  deleteMap(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }

  setActiveMap(campaignId: number, mapId: number): Observable<any> {
    return this.http.put<any>(
      `${this.campaignApiUrl}/${campaignId}/set-active-map/${mapId}`,
      {},
      { headers: this.getHeaders() }
    );
  }
}