import { Component, Output, EventEmitter, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-field-builder', // Verifique se este seletor é o mesmo do HTML
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './field-builder.html',
  styleUrl: './field-builder.scss'
})
export class FieldBuilder implements OnChanges {
  // A falta desta linha causa o erro NG8002 no HTML do pai
  @Input() initialFields: any[] = []; 
  
  @Output() onSchemaChange = new EventEmitter<any[]>();

  fields: any[] = [];

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['initialFields'] && changes['initialFields'].currentValue) {
      this.fields = [...changes['initialFields'].currentValue];
    }
  }

  addField() {
    this.fields.push({ name: '', type: 'number' });
    this.onSchemaChange.emit(this.fields);
  }

  removeField(index: number) {
    this.fields.splice(index, 1);
    this.onSchemaChange.emit(this.fields);
  }
}