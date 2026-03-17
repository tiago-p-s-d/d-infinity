import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { CommonModule, Location } from '@angular/common';
import { Header } from '../../layout/header/header';

@Component({
  selector: 'app-systems',
  imports: [Header, CommonModule, ReactiveFormsModule],
  templateUrl: './systems.html',
  styleUrl: './systems.scss',
})
export class Systems {
  systems: any[] = [];
  systemForm!: FormGroup;
  isAdding = false;

  constructor(private fb: FormBuilder, private http: HttpClient, private location: Location) { }

  ngOnInit() {
    this.initForm();
    this.loadSystems();
  }

  initForm() {
    this.systemForm = this.fb.group({
      name: ['', [Validators.required]],
      characterSheetId: [0, Validators.required],
      itemsId: [0, Validators.required],
      spellsId: [0, Validators.required],
      skillsId: [0, Validators.required],
      mapsId: [0, Validators.required],
      currencyId: [0, Validators.required],
      racesId: [0, Validators.required],
    });
  }

  onSubmit() {
    if (this.systemForm.valid) {
      const systemData = {
        ...this.systemForm.value,
      };

      this.http.post(`${environment.apiUrl}/api/system`, systemData).subscribe({
        next: (res) => {
          this.isAdding = false;
          this.loadSystems();
          this.systemForm.reset({
            // Resetamos com valores padrão de 0 para os campos obrigatórios
            characterSheetId: 0,
            itemsId: 0,
            spellsId: 0,
            skillsId: 0,
            mapsId: 0,
            currencyId: 0,
            racesId: 0
          });
        },
        error: (err) => console.error('Error saving system:', err)
      });
    }
  }

  loadSystems() {
    this.http.get<any[]>(`${environment.apiUrl}/api/system`).subscribe({
      next: (data) => this.systems = data,
      error: (err) => console.error('Error loading systems:', err)
    });
  }

  goBack() {
    this.location.back();
  }


}
