import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class CampaignService {
  private apiUrl = 'http://localhost:5000/api/campaign'; 

  constructor(private http: HttpClient) {}

  startYourOwn(data: any) {
  return this.http.post<{id: number, message: string}>(`${this.apiUrl}/start-your-own`, data);
}
}
