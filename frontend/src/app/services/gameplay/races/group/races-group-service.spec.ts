import { TestBed } from '@angular/core/testing';

import { RacesGroupService } from './races-group-service';

describe('RacesGroupService', () => {
  let service: RacesGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RacesGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
