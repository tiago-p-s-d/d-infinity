import { Component, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MapGroupService } from '../../../services/gameplay/maps/group/maps-group-service';
import { MapService } from '../../../services/gameplay/maps/map-service';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { Header } from '../../layout/header/header';



@Component({
  selector: 'app-maps',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, EditButton, DeleteButton, Header],
  templateUrl: './maps.html',
  styleUrl: './maps.scss',
})
export class Maps implements OnInit {
  mapForm: FormGroup;

  selectedImage = signal<string | null>(null);
  zoomLevel = signal<number>(1);
  gridSize = signal<number>(50);
  showGroupModal = false;
  isEditing = false;

  mapGroups = signal<any[]>([]);
  maps = signal<any[]>([]);

  private fb = inject(FormBuilder);
  private mapGroupService = inject(MapGroupService);
  private mapService = inject(MapService);

  constructor() {
    this.mapForm = this.fb.group({
      name: ['', Validators.required],
      mapGroupId: [null, Validators.required] 
    });
  }

  ngOnInit() {
    this.loadGroups();
    this.loadMaps(); 
  }

  loadMaps() {
    this.mapService.getMaps().subscribe({
      next: (data) => {
        this.maps.set(data);
      },
      error: (err) => {
        alert('Error loading maps');
      }
    });
  }

  loadGroups() {
    this.mapGroupService.getGroups().subscribe({
      next: (groups) => this.mapGroups.set(groups),
      error: (err) => {
        alert('Error loading map groups');
      }
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => this.selectedImage.set(e.target?.result as string);
      reader.readAsDataURL(file);
    }
  }

  adjustZoom(delta: number) {
    this.zoomLevel.update(z => Math.max(0.5, Math.min(3, z + delta)));
  }

  saveNewGroup(name: string) {
    if (!name) return;
    this.mapGroupService.createGroup(name).subscribe({
      next: (group: any) => {
        this.loadGroups(); 
        this.showGroupModal = false;
        this.mapForm.patchValue({ mapGroupId: group.id });
      },
      error: (err) => {
        alert('Error creating map group');
      }
    });
  }

  onSubmit() {
    if (this.mapForm.valid && this.selectedImage()) {
      const payload = {
        ...this.mapForm.value,
        mapImage: this.selectedImage(),
      };

      this.mapService.createMap(payload).subscribe({
        next: () => {
          this.loadMaps();
          this.resetForm();
        },
        error: (err) => {
          alert('Error saving map');
        }
      });
    }
  }

  resetForm() {
    this.mapForm.reset();
    this.selectedImage.set(null);
    this.isEditing = false;
  }

  onEdit(map: any) {
    this.isEditing = true;
    this.mapForm.patchValue(map);
    this.selectedImage.set(map.mapImage);
  }

  onDelete(map: any) {
    if (confirm(`Are you sure you want to delete the map "${map.name}"?`)) {
      this.mapService.deleteMap(map.id).subscribe({
        next: () => {
          this.loadMaps();
        },
        error: (err) => {
          alert('Error deleting map');
        }
      });
    }
  }
}