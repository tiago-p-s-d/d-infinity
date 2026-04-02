import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MapService {
  private apiUrl = `${environment.apiUrl}/Maps`;

  constructor(private http: HttpClient) { }

  getMaps(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getMapById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createMap(map: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, map);
  }

  deleteMap(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
