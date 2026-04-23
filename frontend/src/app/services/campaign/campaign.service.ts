import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CampaignService {
  private apiUrl = `${environment.apiUrl}/campaign`;
  private sheetUrl = `${environment.apiUrl}/character-sheet`;
  private sheetModelUrl = `${environment.apiUrl}/character-sheet-model`;

  constructor(private http: HttpClient) { }

  getCampaigns(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getMyCampaignsAsPlayer(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my-joined-campaigns`);
  }

  createCampaign(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/create`, payload);
  }

  updateCampaign(id: number, payload: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, payload);
  }

  deleteCampaign(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  joinCampaign(inviteCode: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/join/${inviteCode}`, {});
  }

  getSheetModelBySystem(systemId: number): Observable<any> {
    return this.http.get<any>(`${this.sheetModelUrl}/by-system/${systemId}`);
  }

  getSheetByCampaign(campaignId: number): Observable<any> {
    return this.http.get<any>(`${this.sheetUrl}/campaign/${campaignId}`);
  }

  updateSheet(id: number, payload: any): Observable<any> {
    return this.http.put(`${this.sheetUrl}/${id}`, payload);
  }
}