import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ItemService } from '../../../services/gameplay/items/item-service';
import { ItemGroupService } from '../../../services/gameplay/items/group/item-group-service';
import { Header } from '../../layout/header/header';
import { EditButton } from '../../layout/edit-button/edit-button';
import { DeleteButton } from '../../layout/delete-button/delete-button';
import { FieldBuilder } from '../../global/field-builder/field-builder';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    Header,
    EditButton,
    DeleteButton,
    FieldBuilder
  ],
  templateUrl: './items.html',
  styleUrl: './items.scss',
})
export class Items implements OnInit {
  itemForm!: FormGroup;
  items = signal<any[]>([]);
  itemGroups = signal<any[]>([]); // Signal para os grupos

  isEditing = false;
  editingItemId: number | null = null;
  showGroupModal = false; // Controle do modal
  currentFields: any[] = [];

  constructor(
    private fb: FormBuilder,
    private itemService: ItemService,
    private groupService: ItemGroupService // Injeção do service de grupos
  ) { }

  ngOnInit(): void {
    this.initForm();
    this.loadItems();
    this.loadGroups();
  }

  initForm() {
    this.itemForm = this.fb.group({
      name: ['', [Validators.required]],
      description: [''],
      itemGroupId: [null, [Validators.required]] // Campo de grupo obrigatório
    });
  }

  loadGroups() {
    this.groupService.getGroups().subscribe(data => this.itemGroups.set(data));
  }

  loadItems() {
    this.itemService.getItems().subscribe({
      next: (data) => this.items.set(data),
      error: (err) => console.error('Error fetching items', err)
    });
  }

  parseDefinitions(definitions: string) {
    try {
      return definitions ? JSON.parse(definitions) : [];
    } catch (e) {
      return [];
    }
  }

  saveNewGroup(name: string) {
    if (!name) return;
    this.groupService.createGroup(name).subscribe({
      next: (newGroup) => {
        this.itemGroups.update(list => [...list, newGroup]);
        this.itemForm.patchValue({ itemGroupId: newGroup.id });
        this.showGroupModal = false;
      },
      error: (err) => alert("Error creating group")
    });
  }

  updateSchema(fields: any[]) {
    this.currentFields = fields;
  }

  onEdit(item: any) {
    this.isEditing = true;
    this.editingItemId = item.id;

    this.itemForm.patchValue({
      name: item.name,
      description: item.description,
      itemGroupId: item.itemGroupId
    });

    try {
      this.currentFields = item.definitions ? JSON.parse(item.definitions) : [];
    } catch (e) {
      this.currentFields = [];
    }

    document.getElementById('itemFormSection')?.scrollIntoView({ behavior: 'smooth' });
  }

  resetForm() {
    this.isEditing = false;
    this.editingItemId = null;
    this.currentFields = [];
    this.itemForm.reset();
    window.scrollTo({ top: 0, behavior: 'smooth' });
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
      const raw = this.itemForm.value;

      const payload: any = {
        name: raw.name,
        description: raw.description,
        itemGroupId: raw.itemGroupId,
        definitions: JSON.stringify(this.currentFields)
      };

      const request = this.isEditing && this.editingItemId
        ? this.itemService.updateItem(this.editingItemId, payload)
        : this.itemService.createItem(payload);

      request.subscribe({
        next: (res: any) => {
          if (this.isEditing) {
            this.items.update(list => list.map(i => i.id === this.editingItemId ? res : i));
          } else {
            this.items.update(list => [res, ...list]);
          }
          this.resetForm();
        },
        error: (err) => console.error("Error saving item:", err)
      });
    }
  }
}