import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, Location } from '@angular/common';
import { Header } from '../../layout/header/header';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../../environments/environment';



import { ItemGroupService } from '../../../services/gameplay/items/group/item-group-service';
import { SpellGroupService } from '../../../services/gameplay/spells/group/spells-group-service';
import { RacesGroupService } from '../../../services/gameplay/races/group/races-group-service';
import { SkillGroupService } from '../../../services/gameplay/skills/group/skill-group-service';
import { MapGroupService } from '../../../services/gameplay/maps/group/maps-group-service';
import { ClassGroupService } from '../../../services/gameplay/classes/group/class-group-service';
import { SheetService } from '../../../services/gameplay/character-sheet/sheet-service';
import { CurrencyService } from '../../../services/gameplay/currency/currency-service';

import { SystemService } from '../../../services/gameplay/systems/system-service';

@Component({
  selector: 'app-systems',
  standalone: true,
  imports: [Header, CommonModule, ReactiveFormsModule],
  templateUrl: './systems.html',
  styleUrl: './systems.scss',
})
export class Systems implements OnInit {
  systems = signal<any[]>([]);
  systemForm!: FormGroup;
  isAdding = false;

  sheets = signal<any[]>([]);
  currencies = signal<any[]>([]);
  itemsList = signal<any[]>([]);
  spellsList = signal<any[]>([]);
  racesList = signal<any[]>([]);
  skillsList = signal<any[]>([]);

  mapsList = signal<any[]>([]);
  classesList = signal<any[]>([]);

  

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private sheetService: SheetService,
    private currencyService: CurrencyService,
    private itemGroupService: ItemGroupService,
    private spellGroupService: SpellGroupService,
    private raceGroupService: RacesGroupService,
    private skillGroupService: SkillGroupService,
    private mapGroupService: MapGroupService,
    private classGroupService: ClassGroupService,
    private systemService: SystemService
  ) { }

  ngOnInit() {
    this.initForm();
    this.loadSystems();
    this.loadAllPacks();
  }

  private getHeaders() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ 'Authorization': `Bearer ${token}` });
  }

  initForm() {
    this.systemForm = this.fb.group({
      name: ['', [Validators.required]],
      characterSheetId: [null, Validators.required],
      currencyId: [null, Validators.required],
      itemsId: [[]],
      spellsId: [[]],
      skillsId: [[]],
      racesId: [[]],
      mapsId: [[]],
      classesId: [null]
    });
  }

  loadAllPacks() {

    this.sheetService.getsheets().subscribe(data => this.sheets.set(data));
    this.currencyService.getCurrencies().subscribe(data => this.currencies.set(data));
    this.mapGroupService.getGroups().subscribe(data => this.mapsList.set(data));
    this.itemGroupService.getGroups().subscribe(data => this.itemsList.set(data));
    this.spellGroupService.getGroups().subscribe(data => this.spellsList.set(data));
    this.raceGroupService.getGroups().subscribe(data => this.racesList.set(data));
    this.skillGroupService.getGroups().subscribe(data => this.skillsList.set(data));
    this.classGroupService.getGroups().subscribe(data => this.classesList.set(data));
  }

  loadSystems() {
  this.systemService.getSystems().subscribe({
    next: (data) => {
      this.systems.set(data);
      console.log('Signal atualizado:', this.systems());
    },
    error: (err) => console.error('Error loading systems:', err)
  });
}

  onSubmit() {
    if (this.systemForm.valid) {
      const rawValue = this.systemForm.value;
      const payload = {
        ...rawValue,
        itemsId: rawValue.itemsId?.map(Number) || [],
        spellsId: rawValue.spellsId?.map(Number) || [],
        skillsId: rawValue.skillsId?.map(Number) || [],
        racesId: rawValue.racesId?.map(Number) || [],
        mapsId: rawValue.mapsId?.map(Number) || [],
        characterSheetId: Number(rawValue.characterSheetId),
        currencyId: Number(rawValue.currencyId),
        classesId: rawValue.classesId ? Number(rawValue.classesId) : null
      };

      this.systemService.createSystem(payload).subscribe({
        next: () => {
          this.isAdding = false;
          this.loadSystems();
          this.resetForm();
        },
        error: (err) => {
          console.error('Error saving system:', err);
          alert('Error saving system. Verify the console.');
        }
      });
    } else {

      Object.values(this.systemForm.controls).forEach(control => {
        if (control.invalid) control.markAsTouched();
      });
    }
  }

  resetForm() {
    this.systemForm.reset({
      characterSheetId: null,
      currencyId: null,
      itemsId: [],
      spellsId: [],
      skillsId: [],
      racesId: [],
      mapsId: [],
      classesId: null
    });
  }
}