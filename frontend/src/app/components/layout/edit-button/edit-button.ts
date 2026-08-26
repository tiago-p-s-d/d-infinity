import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-edit-button',
  imports: [],
  templateUrl: './edit-button.html',
  styleUrl: './edit-button.scss',
})
export class EditButton {
  @Output() onClick = new EventEmitter<MouseEvent>();
}
