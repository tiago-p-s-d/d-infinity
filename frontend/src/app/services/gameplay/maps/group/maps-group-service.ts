import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MapGroupService {
  // Ajuste a rota de acordo com o seu Controller no .NET (ex: /api/mapgroups ou /api/map/groups)
  private apiUrl = `${environment.apiUrl}/Maps/groups`;

  constructor(private http: HttpClient) {}

  private getHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  /**
   * Busca todos os grupos de mapas
   */
  getGroups(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  /**
   * Cria um novo grupo de mapas
   * @param name Nome do grupo (ex: "Sword Coast", "Dungeon Level 1")
   */
  createGroup(name: string): Observable<any> {
    // Enviamos como objeto para o C# conseguir fazer o Bind do [FromBody]
    return this.http.post<any>(this.apiUrl, { name }, { headers: this.getHeaders() });
  }

  /**
   * Opcional: Busca um grupo específico por ID
   */
  getGroupById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}