import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ItemService } from '../../../services/gameplay/items/item-service';
import { Header } from '../../layout/header/header';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    Header,
    EditButton,
    DeleteButton
  ],
  templateUrl: './items.html',
  styleUrl: './items.scss',
})
export class Items implements OnInit {
  itemForm!: FormGroup;
  items = signal<any[]>([]);

  isEditing = false;
  editingItemId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private itemService: ItemService 
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadItems();
  }

  initForm() {
    this.itemForm = this.fb.group({
      name: ['', [Validators.required]],
      description: [''],
      modifier: [''],
      damage: [''],
      ac: [null]
    });
  }

  loadItems() {
    this.itemService.getItems().subscribe({
      next: (data) => {
        this.items.set(data);
      },
      error: (err) => console.error('Error fetching items', err)
    });
  }

  onEdit(item: any) {
    this.isEditing = true;
    this.editingItemId = item.id;
    
    this.itemForm.patchValue({
      name: item.name,
      description: item.description,
      modifier: item.modifier,
      damage: item.damage,
      ac: item.ac
    });

    document.getElementById('itemFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  cancelEdit() {
    this.isEditing = false;
    this.editingItemId = null;
    this.itemForm.reset();
  }

  onDelete(item: any) {
    if (confirm(`Are you sure you want to delete "${item.name}"?`)) {
      this.itemService.deleteItem(item.id).subscribe({
        next: () => {
          this.items.update(prev => prev.filter(i => i.id !== item.id));
        },
        error: (err) => alert('Error deleting item: ' + err.message)
      });
    }
  }

  onSubmit() {
    if (this.itemForm.valid) {
      const itemData = this.itemForm.value;

      if (this.isEditing && this.editingItemId) {
        this.itemService.updateItem(this.editingItemId, itemData).subscribe({
          next: (updatedItem) => {
            this.items.update(prev => 
              prev.map(i => i.id === this.editingItemId ? updatedItem : i)
            );
            this.cancelEdit(); 
          },
          error: (err) => alert('Error updating item: ' + err.message)
        });
      } else {
        this.itemService.createItem(itemData).subscribe({
          next: (newItem) => {
            this.items.update(prev => [newItem, ...prev]);
            this.itemForm.reset();
          },
          error: (err) => alert('Error saving item: ' + err.message)
        });
      }
    }
  }
}