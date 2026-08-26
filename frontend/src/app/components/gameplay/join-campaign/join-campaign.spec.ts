import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinCampaign } from './join-campaign';

describe('JoinCampaign', () => {
  let component: JoinCampaign;
  let fixture: ComponentFixture<JoinCampaign>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JoinCampaign]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JoinCampaign);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
