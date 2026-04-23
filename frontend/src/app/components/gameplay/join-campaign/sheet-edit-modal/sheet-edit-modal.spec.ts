import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SheetEditModal } from './sheet-edit-modal';

describe('SheetEditModal', () => {
  let component: SheetEditModal;
  let fixture: ComponentFixture<SheetEditModal>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SheetEditModal]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SheetEditModal);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
