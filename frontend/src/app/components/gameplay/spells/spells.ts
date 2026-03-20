import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SpellService } from '../../../services/gameplay/spells/spell-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';


@Component({
  selector: 'app-spells',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton],
  templateUrl: './spells.html',
  styleUrl: './spells.scss',
})
export class Spells implements OnInit {
  spellForm!: FormGroup;
    spells = signal<any[]>([]);
    isEditing = false;
    editingspellId: number | null = null;
  
    constructor(private fb: FormBuilder, private spellService: SpellService) {}
  
    ngOnInit(): void {
      this.initForm();
      this.loadspells();
    }
  
    initForm() {
      this.spellForm = this.fb.group({
        name: ['', [Validators.required]],
        about: [''],
        customStat: [''],
        customValue: [0]
      });
    }
  
    loadspells() {
      this.spellService.getSpells().subscribe((data: any[]) => this.spells.set(data));
    }
  
    onSubmit() {
      if (this.spellForm.valid) {
        const raw = this.spellForm.value;
        
        const spellPayload: any = {
          name: raw.name,
          about: raw.about,
          effect: JSON.stringify({
            stat: raw.customStat,
            value: raw.customValue
          })
        };
  
        if (this.isEditing && this.editingspellId) {
          spellPayload.id = this.editingspellId;
  
          this.spellService.update(this.editingspellId, spellPayload).subscribe((res: any) => {
            this.spells.update(list => list.map(s => s.id === this.editingspellId ? res : s));
            this.resetForm();
          });
        } else {
          this.spellService.create(spellPayload).subscribe((res: any) => {
            this.spells.update(list => [res, ...list]);
            this.resetForm();
          });
        }
      }
    }
  
    onEdit(spell: any) {
      this.isEditing = true;
      this.editingspellId = spell.id;
      
      const eff = typeof spell.effect === 'string' ? JSON.parse(spell.effect) : spell.effect;
  
      this.spellForm.patchValue({
        name: spell.name,
        about: spell.about,
        customStat: eff.stat || '',
        customValue: eff.value || 0
      });
  
      document.getElementById('itemFormSection')?.scrollIntoView({ behavior: 'smooth' });
    }
  
    getEffect(jsonStr: string) {
      try {
        return typeof jsonStr === 'string' ? JSON.parse(jsonStr) : jsonStr;
      } catch {
        return { stat: 'N/A', value: 0 };
      }
    }
  
    resetForm() {
      this.isEditing = false;
      this.editingspellId = null;
      this.spellForm.reset({ customValue: 0 });
    }
  
    onDelete(id: number) {
      if (confirm('Delete this spell?')) {
        this.spellService.delete(id).subscribe(() => {
          this.spells.update(list => list.filter(s => s.id !== id));
        });
      }
    }
}
