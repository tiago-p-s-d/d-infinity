import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SkillService } from '../../../services/gameplay/skills/skill-service';
import { SkillGroupService } from '../../../services/gameplay/skills/group/skill-group-service'; 
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { Header } from '../../layout/header/header';
import { FieldBuilder } from '../../global/field-builder/field-builder';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton, Header, FieldBuilder],
  templateUrl: './skills.html',
  styleUrl: './skills.scss',
})
export class Skills implements OnInit {
  skillForm!: FormGroup;
  skills = signal<any[]>([]);
  skillGroups = signal<any[]>([]); 
  
  isEditing = false;
  editingSkillId: number | null = null;
  showGroupModal = false;
  currentFields: any[] = []; 

  constructor(
    private fb: FormBuilder, 
    private skillService: SkillService,
    private groupService: SkillGroupService 
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadSkills();
    this.loadGroups();
  }

  initForm() {
    this.skillForm = this.fb.group({
      name: ['', [Validators.required]],
      about: [''],
      skillGroupId: [null, [Validators.required]] 
    });
  }

  loadSkills() {
    this.skillService.getSkills().subscribe((data: any[]) => this.skills.set(data));
  }

  loadGroups() {
    this.groupService.getGroups().subscribe(data => this.skillGroups.set(data));
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
        this.skillGroups.update(list => [...list, newGroup]);
        this.skillForm.patchValue({ skillGroupId: newGroup.id });
        this.showGroupModal = false;
      },
      error: (err) => alert("Error creating skill group")
    });
  }

  onSubmit() {
    if (this.skillForm.valid) {
      const raw = this.skillForm.value;
      
      const skillPayload: any = {
        name: raw.name,
        about: raw.about,
        skillGroupId: raw.skillGroupId,
        effect: JSON.stringify(this.currentFields) 
      };

      const request = this.isEditing && this.editingSkillId
        ? this.skillService.update(this.editingSkillId, skillPayload)
        : this.skillService.create(skillPayload);

      request.subscribe({
        next: (res: any) => {
          this.loadSkills(); 
          this.resetForm();
        },
        error: (err) => console.error("Error saving skill:", err)
      });
    }
  }

  onEdit(skill: any) {
    this.isEditing = true;
    this.editingSkillId = skill.id;
    
    this.skillForm.patchValue({
      name: skill.name,
      about: skill.about,
      skillGroupId: skill.skillGroupId
    });

    try {
      this.currentFields = skill.effect ? JSON.parse(skill.effect) : [];
    } catch (e) {
      this.currentFields = [];
    }

    document.getElementById('skillFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  resetForm() {
    this.isEditing = false;
    this.editingSkillId = null;
    this.currentFields = [];
    this.skillForm.reset();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDelete(skill: any) {
    if (confirm(`Delete skill "${skill.name}"?`)) {
      this.skillService.delete(skill.id).subscribe(() => {
        this.skills.update(list => list.filter(s => s.id !== skill.id));
      });
    }
  }
}