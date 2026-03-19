import { Component, signal, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-maps',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './maps.html',
  styleUrl: './maps.scss',
})
export class Maps {
mapForm: FormGroup;
  selectedImage = signal<string | null>(null);
  zoomLevel = signal<number>(1);
  gridSize = signal<number>(50); 

  constructor(private fb: FormBuilder) {
    this.mapForm = this.fb.group({
      name: ['', Validators.required],
      campaignId: [null, Validators.required]
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

  onSubmit() {
    if (this.mapForm.valid && this.selectedImage()) {
      const payload = {
        ...this.mapForm.value,
        mapImage: this.selectedImage(),
      };
      console.log('Sending Map to C#:', payload);
    }
  }
}
