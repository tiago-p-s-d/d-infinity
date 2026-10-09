import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GameObjects } from './game-objects';

describe('GameObjects', () => {
  let component: GameObjects;
  let fixture: ComponentFixture<GameObjects>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GameObjects]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GameObjects);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
