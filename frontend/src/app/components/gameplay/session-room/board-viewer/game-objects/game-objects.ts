import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MapToken } from '../../../../../interfaces/map-token.model';

@Component({
  selector: 'app-game-objects',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './game-objects.html',
  styleUrl: './game-objects.scss',
})
export class GameObjects {
  tokens = input<MapToken[]>([]);
  currentUserId = input<number>(0);
  isDm = input<boolean>(false);
  cellSize = input<number>(50);
 
  zoom = input<number>(1.0);

  tokenMoved = output<{ tokenId: number; coordX: number; coordY: number }>();
  tokenResized = output<{ tokenId: number; size: number }>();

  // Moving authorization: DM can move all tokens; players only their own
  canMove(token: MapToken): boolean {
    return this.isDm() || (token.ownerUserId === this.currentUserId() && this.currentUserId() > 0);
  }

  onMouseDown(event: MouseEvent, token: MapToken): void {
    if (event.button !== 0 || !this.canMove(token)) return;

    event.preventDefault();
    event.stopPropagation();

    const startX = event.clientX;
    const startY = event.clientY;
    const initialCoordX = token.coordX;
    const initialCoordY = token.coordY;
    const currentZoom = Math.max(0.1, this.zoom());

    let currentX = initialCoordX;
    let currentY = initialCoordY;

    const onMouseMove = (moveEvent: MouseEvent) => {
      const deltaX = (moveEvent.clientX - startX) / currentZoom;
      const deltaY = (moveEvent.clientY - startY) / currentZoom;

      currentX = Math.max(0, Math.round(initialCoordX + deltaX));
      currentY = Math.max(0, Math.round(initialCoordY + deltaY));

      // Visual feedback during movement
      token.coordX = currentX;
      token.coordY = currentY;
    };

    const onMouseUp = () => {
      window.removeEventListener('mousemove', onMouseMove);
      window.removeEventListener('mouseup', onMouseUp);

     
      const snapped = this.snapToGrid(currentX, currentY);
      token.coordX = snapped.x;
      token.coordY = snapped.y;

      this.tokenMoved.emit({
        tokenId: token.id,
        coordX: snapped.x,
        coordY: snapped.y,
      });
    };

    window.addEventListener('mousemove', onMouseMove);
    window.addEventListener('mouseup', onMouseUp);
  }

  // Snaps coordinate values to the active grid step
  private snapToGrid(x: number, y: number): { x: number; y: number } {
    const size = this.cellSize();
    if (size <= 0) return { x, y };

    return {
      x: Math.round(x / size) * size,
      y: Math.round(y / size) * size,
    };
  }

}
