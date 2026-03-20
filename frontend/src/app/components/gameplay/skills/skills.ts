import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SkillService } from '../../../services/gameplay/skills/skill-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';

@Component({
  selector: 'app-skills',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton],
  templateUrl: './skills.html',
  styleUrl: './skills.scss',
})
export class Skills implements OnInit {
  skillForm!: FormGroup;
  skills = signal<any[]>([]);
  isEditing = false;
  editingSkillId: number | null = null;

  constructor(private fb: FormBuilder, private skillService: SkillService) {}

  ngOnInit(): void {
    this.initForm();
    this.loadSkills();
  }

  initForm() {
    this.skillForm = this.fb.group({
      name: ['', [Validators.required]],
      about: [''],
      customStat: [''],
      customValue: [0]
    });
  }

  loadSkills() {
    this.skillService.getSkills().subscribe((data: any[]) => this.skills.set(data));
  }

  onSubmit() {
    if (this.skillForm.valid) {
      const raw = this.skillForm.value;
      
      const skillPayload: any = {
        name: raw.name,
        about: raw.about,
        effect: JSON.stringify({
          stat: raw.customStat,
          value: raw.customValue
        })
      };

      if (this.isEditing && this.editingSkillId) {
        skillPayload.id = this.editingSkillId;

        this.skillService.update(this.editingSkillId, skillPayload).subscribe((res: any) => {
          this.skills.update(list => list.map(s => s.id === this.editingSkillId ? res : s));
          this.resetForm();
        });
      } else {
        this.skillService.create(skillPayload).subscribe((res: any) => {
          this.skills.update(list => [res, ...list]);
          this.resetForm();
        });
      }
    }
  }

  onEdit(skill: any) {
    this.isEditing = true;
    this.editingSkillId = skill.id;
    
    const eff = typeof skill.effect === 'string' ? JSON.parse(skill.effect) : skill.effect;

    this.skillForm.patchValue({
      name: skill.name,
      about: skill.about,
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
    this.editingSkillId = null;
    this.skillForm.reset({ customValue: 0 });
  }

  onDelete(id: number) {
    if (confirm('Delete this skill?')) {
      this.skillService.delete(id).subscribe(() => {
        this.skills.update(list => list.filter(s => s.id !== id));
      });
    }
  }
}
