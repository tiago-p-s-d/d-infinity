import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FieldBuilder } from './field-builder';

describe('FieldBuilder', () => {
  let component: FieldBuilder;
  let fixture: ComponentFixture<FieldBuilder>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FieldBuilder]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FieldBuilder);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
