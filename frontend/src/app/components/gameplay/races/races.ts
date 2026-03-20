import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormatterUtils } from '../../../utils/formatters';
import { RaceService } from '../../../services/gameplay/races/race-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';

@Component({
  selector: 'app-races',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton],
  templateUrl: './races.html',
  styleUrl: './races.scss',
})
export class Races implements OnInit {
  raceForm!: FormGroup;
  races = signal<any[]>([]);
  isEditing = false;
  editingRaceId: number | null = null;

  constructor(private fb: FormBuilder, private RaceService: RaceService) { }

  ngOnInit(): void {
    this.initForm();
    this.loadRaces();
  }

  initForm() {
    this.raceForm = this.fb.group({
      name: ['', [Validators.required]],
      about: [''],
      modifiers: [''] 
    });
  }
  loadRaces() {
    this.RaceService.getRaces().subscribe((data: any[]) => this.races.set(data));
  }

  onSubmit() {
    if (this.raceForm.valid) {
      const raw = this.raceForm.value;

      const racePayload: any = {
        name: raw.name,
        about: raw.about,
        modifiers: FormatterUtils.parseInputToJson(raw.modifiers)
      };

      if (this.isEditing && this.editingRaceId) {
        racePayload.id = this.editingRaceId;
        this.RaceService.update(this.editingRaceId, racePayload).subscribe((res: any) => {
          this.races.update(list => list.map(r => r.id === this.editingRaceId ? res : r));
          this.resetForm();
        });
      } else {
        this.RaceService.create(racePayload).subscribe((res: any) => {
          this.races.update(list => [res, ...list]);
          this.resetForm();
        });
      }
    }
  }

  onEdit(race: any) {
    this.isEditing = true;
    this.editingRaceId = race.id;

    this.raceForm.patchValue({
      name: race.name,
      about: race.about,
      modifiers: race.FormatterUtils.parseInputToJson(race.modifiers)
    });

    document.getElementById('raceFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  getModifiers(jsonStr: string) {
    try {
      const mods = typeof jsonStr === 'string' ? JSON.parse(jsonStr) : jsonStr;
      return Object.entries(mods).filter(([key]) => key !== '');
    } catch {
      return [];
    }
  }

  resetForm() {
    this.isEditing = false;
    this.editingRaceId = null;
    this.raceForm.reset({ statValue: 0, secondaryValue: 0 });
  }

  onDelete(race: any) {
    if (confirm(`Delete race "${race.name}"?`)) {
      this.RaceService.delete(race.id).subscribe(() => {
        this.races.update(list => list.filter(r => r.id !== race.id));
      });
    }
  }
}
