import { Component, OnInit } from '@angular/core'; // Adicionado OnInit
import { ActivatedRoute, Router } from '@angular/router'; // Adicionado ActivatedRoute e Router
import { HttpClient } from '@angular/common/http'; // Adicionado HttpClient
import { environment } from '../../../../environments/environment'; // Verifique se o caminho está correto
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-invite-handler',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './invite-handler.html',
  styleUrl: './invite-handler.scss',
})
export class InviteHandler implements OnInit {
  inviteCode: string = '';
  campaignData: any = null;

  constructor(
    private route: ActivatedRoute, 
    private http: HttpClient, 
    private router: Router
  ) {}

  ngOnInit() {
    // Captura o código da URL (ex: /invite/:code)
    this.inviteCode = this.route.snapshot.params['code'];
    
    if (this.inviteCode) {
      this.loadInviteInfo();
    }
  }

  // Método que estava faltando
  loadInviteInfo() {
    // Endpoint opcional para o player ver o nome da campanha antes de aceitar
    // Se você não criou esse endpoint no C#, pode apenas comentar a chamada no ngOnInit
    this.http.get(`${environment.apiUrl}/api/campaign/invite-info/${this.inviteCode}`).subscribe({
      next: (data) => this.campaignData = data,
      error: (err) => console.error('Erro ao buscar dados do convite', err)
    });
  }

  acceptInvite() {
    this.http.post(`${environment.apiUrl}/api/campaign/join/${this.inviteCode}`, {}).subscribe({
      next: (res: any) => {
        // Redireciona para a criação de ficha (ajuste a rota conforme seu projeto)
        this.router.navigate(['/sheet-builder', res.systemId], { 
          queryParams: { campaign: res.campaignId } 
        });
      },
      error: (err) => {
        console.error(err);
        alert('Erro ao entrar na campanha ou convite expirado.');
      }
    });
  }

  decline() {
    this.router.navigate(['/home']);
  }
}