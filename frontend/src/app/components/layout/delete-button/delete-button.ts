import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-delete-button',
  imports: [],
  templateUrl: './delete-button.html',
  styleUrl: './delete-button.scss',
})
export class DeleteButton {
@Output() onClick = new EventEmitter<MouseEvent>();
}
