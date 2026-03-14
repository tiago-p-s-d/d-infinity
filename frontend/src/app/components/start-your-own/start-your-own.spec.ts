import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StartYourOwn } from './start-your-own';

describe('StartYourOwn', () => {
  let component: StartYourOwn;
  let fixture: ComponentFixture<StartYourOwn>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StartYourOwn]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StartYourOwn);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
