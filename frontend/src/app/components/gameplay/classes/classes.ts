import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClassService } from '../../../services/gameplay/classes/class-service';
import { ClassGroupService } from '../../../services/gameplay/classes/group/class-group-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { Header } from '../../layout/header/header';
import { FieldBuilder } from '../../global/field-builder/field-builder';

@Component({
  selector: 'app-classes',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton, Header, FieldBuilder],
  templateUrl: './classes.html',
  styleUrl: './classes.scss',
})
export class Classes implements OnInit {
  classForm!: FormGroup;
  classes = signal<any[]>([]);
  classGroups = signal<any[]>([]);
  
  isEditing = false;
  editingClassId: number | null = null;
  showGroupModal = false;
  currentFields: any[] = []; // Para o FieldBuilder (JSON definitions)

  constructor(
    private fb: FormBuilder, 
    private classService: ClassService,
    private groupService: ClassGroupService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadClasses();
    this.loadGroups();
  }

  initForm() {
    this.classForm = this.fb.group({
      name: ['', [Validators.required]],
      about: [''],
      classGroupId: [null, [Validators.required]]
    });
  }

  loadClasses() {
    this.classService.getClasses().subscribe((data: any[]) => this.classes.set(data));
  }

  loadGroups() {
    this.groupService.getGroups().subscribe(data => this.classGroups.set(data));
  }

  updateSchema(fields: any[]) {
    this.currentFields = fields;
  }

  parseDefinitions(json: string) {
    try {
      return json ? JSON.parse(json) : [];
    } catch (e) {
      return [];
    }
  }

  saveNewGroup(name: string) {
    if (!name) return;
    this.groupService.createGroup(name).subscribe({
      next: (newGroup) => {
        this.classGroups.update(list => [...list, newGroup]);
        this.classForm.patchValue({ classGroupId: newGroup.id });
        this.showGroupModal = false;
      },
      error: (err) => alert("Error creating class group")
    });
  }

  onSubmit() {
    if (this.classForm.valid) {
      const raw = this.classForm.value;
      
      const payload: any = {
        name: raw.name,
        about: raw.about,
        classGroupId: raw.classGroupId,
        definitions: JSON.stringify(this.currentFields)
      };

      const request = this.isEditing && this.editingClassId
        ? this.classService.update(this.editingClassId, payload)
        : this.classService.create(payload);

      request.subscribe({
        next: () => {
          this.loadClasses();
          this.resetForm();
        },
        error: (err) => console.error("Error saving class:", err)
      });
    }
  }

  onEdit(cls: any) {
    this.isEditing = true;
    this.editingClassId = cls.id;
    
    this.classForm.patchValue({
      name: cls.name,
      about: cls.about,
      classGroupId: cls.classGroupId
    });

    try {
      this.currentFields = cls.definitions ? JSON.parse(cls.definitions) : [];
    } catch (e) {
      this.currentFields = [];
    }

    document.getElementById('classFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  resetForm() {
    this.isEditing = false;
    this.editingClassId = null;
    this.currentFields = [];
    this.classForm.reset();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(cls: any) {
    if (confirm(`Delete class "${cls.name}"?`)) {
      this.classService.delete(cls.id).subscribe(() => {
        this.classes.update(list => list.filter(c => c.id !== cls.id));
      });
    }
  }
}