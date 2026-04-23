import { TestBed } from '@angular/core/testing';

import { MapsGroupService } from './maps-group-service';

describe('MapsGroupService', () => {
  let service: MapsGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MapsGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
