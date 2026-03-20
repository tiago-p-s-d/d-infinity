import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SheetService } from '../../../services/gameplay/character-sheet/sheet-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { FieldBuilder } from '../../global/field-builder/field-builder';

@Component({
  selector: 'app-character-sheet',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    DeleteButton, 
    EditButton,
    FieldBuilder // Adicionado aqui
  ],
  templateUrl: './character-sheet.html',
  styleUrl: './character-sheet.scss',
})
export class CharacterSheet implements OnInit {
  sheetForm!: FormGroup;
  sheets = signal<any[]>([]);
  isEditing = false;
  editingId: number | null = null;
  
 
  currentFields: any[] = [];

  constructor(private fb: FormBuilder, private sheetService: SheetService) {}

  ngOnInit(): void {
    this.initForm();
    this.loadsheets();
  }

  initForm() {
    this.sheetForm = this.fb.group({
      name: ['', [Validators.required]]
      // Removi o rawDefinitions daqui pois o FieldBuilder gerencia os campos
    });
  }

  // Função chamada pelo (onSchemaChange) no HTML
  updateSchema(fields: any[]) {
    this.currentFields = fields;
  }

  onSubmit() {
    // Validamos o form e se existe pelo menos um campo definido
    if (this.sheetForm.valid && this.currentFields.length > 0) {
      const raw = this.sheetForm.value;
      
      const payload: any = {
        name: raw.name,
        // Transformamos o array de objetos em JSON string para o backend
        definitions: JSON.stringify(this.currentFields)
      };

      if (this.isEditing && this.editingId) {
        this.sheetService.update(this.editingId, payload).subscribe(res => {
          this.sheets.update(list => list.map(m => m.id === this.editingId ? res : m));
          this.resetForm();
        });
      } else {
        this.sheetService.create(payload).subscribe(res => {
          this.sheets.update(list => [res, ...list]);
          this.resetForm();
        });
      }
    } else if (this.currentFields.length === 0) {
      alert("Please add at least one attribute definition.");
    }
  }

  onEdit(sheet: any) {
    this.isEditing = true;
    this.editingId = sheet.id;
    
    this.sheetForm.patchValue({
      name: sheet.name
    });

    // Passamos os dados salvos de volta para o FieldBuilder
    try {
      this.currentFields = JSON.parse(sheet.definitions);
    } catch (e) {
      this.currentFields = []; // Fallback caso o dado esteja corrompido
    }
  }

  onDelete(sheet: any) {
    if (confirm(`Are you sure you want to delete the system "${sheet.name}"?`)) {
      this.sheetService.delete(sheet.id).subscribe(() => {
        this.sheets.update(list => list.filter(s => s.id !== sheet.id));
      });
    }
  }

  loadsheets() {
    this.sheetService.getsheets().subscribe(data => this.sheets.set(data));
  }

  resetForm() {
    this.isEditing = false;
    this.editingId = null;
    this.currentFields = []; 
    this.sheetForm.reset();
  }
}