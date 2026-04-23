import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SpellService } from '../../../services/gameplay/spells/spell-service';
import { SpellGroupService } from '../../../services/gameplay/spells/group/spells-group-service';// Importe o service de grupos
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { Header } from '../../layout/header/header';
import { FieldBuilder } from '../../global/field-builder/field-builder';

@Component({
  selector: 'app-spells',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton, Header, FieldBuilder],
  templateUrl: './spells.html',
  styleUrl: './spells.scss',
})
export class Spells implements OnInit {
  spellForm!: FormGroup;
  spells = signal<any[]>([]);
  spellGroups = signal<any[]>([]); 
  
  isEditing = false;
  editingSpellId: number | null = null;
  showGroupModal = false;
  currentFields: any[] = []; 

  constructor(
    private fb: FormBuilder, 
    private spellService: SpellService,
    private groupService: SpellGroupService 
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadSpells();
    this.loadGroups();
  }

  initForm() {
    this.spellForm = this.fb.group({
      name: ['', [Validators.required]],
      about: [''],
      spellGroupId: [null, [Validators.required]]
    });
  }

  loadSpells() {
    this.spellService.getSpells().subscribe((data: any[]) => this.spells.set(data));
  }

  loadGroups() {
    this.groupService.getGroups().subscribe(data => this.spellGroups.set(data));
  }

  updateSchema(fields: any[]) {
    this.currentFields = fields;
  }

  parseEffect(effectJson: string) {
    try {
      return effectJson ? JSON.parse(effectJson) : [];
    } catch (e) {
      return [];
    }
  }

  saveNewGroup(name: string) {
    if (!name) return;
    this.groupService.createGroup(name).subscribe({
      next: (newGroup) => {
        this.spellGroups.update(list => [...list, newGroup]);
        this.spellForm.patchValue({ spellGroupId: newGroup.id });
        this.showGroupModal = false;
      },
      error: (err) => alert("Error creating spell group")
    });
  }

  onSubmit() {
    if (this.spellForm.valid) {
      const raw = this.spellForm.value;
      
      const spellPayload: any = {
        name: raw.name,
        about: raw.about,
        spellGroupId: raw.spellGroupId,
        effect: JSON.stringify(this.currentFields) 
      };

      const request = this.isEditing && this.editingSpellId
        ? this.spellService.update(this.editingSpellId, spellPayload)
        : this.spellService.create(spellPayload);

      request.subscribe({
        next: (res: any) => {
          this.loadSpells(); 
          this.resetForm();
        },
        error: (err) => console.error("Error saving spell:", err)
      });
    }
  }

  onEdit(spell: any) {
    this.isEditing = true;
    this.editingSpellId = spell.id;
    
    this.spellForm.patchValue({
      name: spell.name,
      about: spell.about,
      spellGroupId: spell.spellGroupId
    });

    try {
      this.currentFields = spell.effect ? JSON.parse(spell.effect) : [];
    } catch (e) {
      this.currentFields = [];
    }

    document.getElementById('spellFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  resetForm() {
    this.isEditing = false;
    this.editingSpellId = null;
    this.currentFields = [];
    this.spellForm.reset();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(spell: any) {
    if (confirm(`Delete spell "${spell.name}"?`)) {
      this.spellService.delete(spell.id).subscribe(() => {
        this.spells.update(list => list.filter(s => s.id !== spell.id));
      });
    }
  }
}