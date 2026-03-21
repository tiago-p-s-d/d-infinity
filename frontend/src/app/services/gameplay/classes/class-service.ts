import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClassService {
  private apiUrl = `${environment.apiUrl}/api/class`;

  constructor(private http: HttpClient) {}

  getClasses(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  createClass(rpgClass: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, rpgClass);
  }
}