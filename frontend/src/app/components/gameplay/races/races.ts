import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RaceService } from '../../../services/gameplay/races/race-service';
import { RacesGroupService } from '../../../services/gameplay/races/group/races-group-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { Header } from '../../layout/header/header';
import { FieldBuilder } from '../../global/field-builder/field-builder'; 

@Component({
  selector: 'app-races',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    EditButton, 
    DeleteButton, 
    Header, 
    FieldBuilder 
  ],
  templateUrl: './races.html',
  styleUrl: './races.scss',
})
export class Races implements OnInit {
  raceForm!: FormGroup;
  races = signal<any[]>([]);
  raceGroups = signal<any[]>([]);
  
  isEditing = false;
  editingRaceId: number | null = null;
  showGroupModal = false;

  currentFields: any[] = [];

  constructor(
    private fb: FormBuilder, 
    private raceService: RaceService,
    private groupService: RacesGroupService
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadRaces();
    this.loadGroups();
  }

  initForm() {
    this.raceForm = this.fb.group({
      name: ['', [Validators.required]],
      about: [''],
      raceGroupId: [null, [Validators.required]] 
    });
  }

  loadRaces() {
    this.raceService.getRaces().subscribe((data: any[]) => this.races.set(data));
  }

  loadGroups() {
    this.groupService.getGroups().subscribe(data => this.raceGroups.set(data));
  }

  updateSchema(fields: any[]) {
    this.currentFields = fields;
  }

  parseModifiers(modifiersJson: string) {
    try {
      return modifiersJson ? JSON.parse(modifiersJson) : [];
    } catch (e) {
      return [];
    }
  }

  saveNewGroup(name: string) {
    if (!name) return;
    this.groupService.createGroup(name).subscribe({
      next: (newGroup) => {
        this.raceGroups.update(list => [...list, newGroup]);
        this.raceForm.patchValue({ raceGroupId: newGroup.id });
        this.showGroupModal = false;
      },
      error: (err) => alert("Error creating race group")
    });
  }

  onSubmit() {
    if (this.raceForm.valid) {
      const raw = this.raceForm.value;

      const racePayload: any = {
        name: raw.name,
        about: raw.about,
        raceGroupId: raw.raceGroupId,
        modifiers: JSON.stringify(this.currentFields)
      };

      const request = this.isEditing && this.editingRaceId
        ? this.raceService.update(this.editingRaceId, racePayload)
        : this.raceService.create(racePayload);

      request.subscribe({
        next: (res: any) => {
          this.loadRaces();
          this.resetForm();
        },
        error: (err) => console.error("Error saving race:", err)
      });
    }
  }

  onEdit(race: any) {
    this.isEditing = true;
    this.editingRaceId = race.id;

    this.raceForm.patchValue({
      name: race.name,
      about: race.about,
      raceGroupId: race.raceGroupId
    });

    try {
      this.currentFields = race.modifiers ? JSON.parse(race.modifiers) : [];
    } catch (e) {
      this.currentFields = [];
    }

    document.getElementById('raceFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  resetForm() {
    this.isEditing = false;
    this.editingRaceId = null;
    this.currentFields = []; // Limpa o builder
    this.raceForm.reset();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(race: any) {
    if (confirm(`Delete race "${race.name}"?`)) {
      this.raceService.delete(race.id).subscribe(() => {
        this.races.update(list => list.filter(r => r.id !== race.id));
      });
    }
  }
}