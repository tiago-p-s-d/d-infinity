import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, Location } from '@angular/common';
import { Header } from '../../layout/header/header';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

import { SheetService } from '../../../services/gameplay/character-sheet/sheet-service';
import { CurrencyService } from '../../../services/gameplay/currency/currency-service';
import { ItemService } from '../../../services/gameplay/items/item-service';
import { SpellService } from '../../../services/gameplay/spells/spell-service';

import { RaceService } from '../../../services/gameplay/races/race-service';
import { SkillService } from '../../../services/gameplay/skills/skill-service';
import { MapService } from '../../../services/gameplay/maps/map-service';
import { ClassService } from '../../../services/gameplay/classes/class-service'; 

@Component({
  selector: 'app-systems',
  standalone: true,
  imports: [Header, CommonModule, ReactiveFormsModule],
  templateUrl: './systems.html',
  styleUrl: './systems.scss',
})
export class Systems implements OnInit {
  systems: any[] = [];
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
    private location: Location,
    private sheetService: SheetService,
    private currencyService: CurrencyService,
    private itemService: ItemService,
    private spellService: SpellService,
    private raceService: RaceService,
    private skillService: SkillService,
    private mapService: MapService,
    private classService: ClassService 
  ) { }

  ngOnInit() {
    this.initForm();
    this.loadSystems();
    this.loadAllOptions();
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

  loadAllOptions() {
    this.sheetService.getsheets().subscribe(data => this.sheets.set(data));
    this.currencyService.getCurrencies().subscribe(data => this.currencies.set(data));
    this.itemService.getItems().subscribe(data => this.itemsList.set(data));
    this.spellService.getSpells().subscribe(data => this.spellsList.set(data));
    this.raceService.getRaces().subscribe(data => this.racesList.set(data));
    this.skillService.getSkills().subscribe(data => this.skillsList.set(data));
    this.mapService.getMaps().subscribe((data: any[]) => this.mapsList.set(data));
    this.classService.getClasses().subscribe(data => this.classesList.set(data)); 
  }

  loadSystems() {
    this.http.get<any[]>(`${environment.apiUrl}/api/system`).subscribe({
      next: (data) => this.systems = data,
      error: (err) => console.error('Error loading systems:', err)
    });
  }

  onSubmit() {
    if (this.systemForm.valid) {
      const payload = { ...this.systemForm.value };

      console.log('Sending System Payload:', payload);

      this.http.post(`${environment.apiUrl}/api/system`, payload).subscribe({
        next: (res) => {
          this.isAdding = false;
          this.loadSystems();
          this.resetForm();
        },
        error: (err) => console.error('Error saving system:', err)
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

  goBack() {
    this.location.back();
  }
}