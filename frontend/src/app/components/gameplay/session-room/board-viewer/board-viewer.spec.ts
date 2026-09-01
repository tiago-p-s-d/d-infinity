import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BoardViewer } from './board-viewer';

describe('BoardViewer', () => {
  let component: BoardViewer;
  let fixture: ComponentFixture<BoardViewer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BoardViewer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BoardViewer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
