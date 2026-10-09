import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { signal } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { MapToken, CreateMapTokenDto } from '../../../interfaces/map-token.model';

@Injectable({
  providedIn: 'root',
})
export class MapTokenService {
  private apiUrl = `${environment.apiUrl}/MapTokens`;

  tokens = signal<MapToken[]>([]);
  isLoading = signal<boolean>(false);

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    });
  }

  loadTokensByMap(mapId: number): Observable<MapToken[]> {
    this.isLoading.set(true);
    return this.http.get<MapToken[]>(`${this.apiUrl}/map/${mapId}`, { headers: this.getHeaders() }).pipe(
      tap((data) => {
        this.tokens.set(data);
        this.isLoading.set(false);
      })
    );
  }

  createToken(dto: CreateMapTokenDto): Observable<MapToken> {
    return this.http.post<MapToken>(this.apiUrl, dto, { headers: this.getHeaders() }).pipe(
      tap((created) => {
        this.tokens.update((list) => [...list, created]);
      })
    );
  }

  updateTokenPositionInMemory(tokenId: number, coordX: number, coordY: number): void {
    this.tokens.update((list) =>
      list.map((t) => (t.id === tokenId ? { ...t, coordX, coordY } : t))
    );
  }
}