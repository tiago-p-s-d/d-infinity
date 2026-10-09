import { TestBed } from '@angular/core/testing';

import { MapTokenService } from './map-token-service';

describe('MapTokenService', () => {
  let service: MapTokenService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MapTokenService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
