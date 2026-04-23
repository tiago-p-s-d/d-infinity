import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuickTutorial } from './quick-tutorial';

describe('QuickTutorial', () => {
  let component: QuickTutorial;
  let fixture: ComponentFixture<QuickTutorial>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [QuickTutorial]
    })
    .compileComponents();

    fixture = TestBed.createComponent(QuickTutorial);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
