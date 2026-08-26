import { TestBed } from '@angular/core/testing';

import { SpellsGroupService } from './spells-group-service';

describe('SpellsGroupService', () => {
  let service: SpellsGroupService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SpellsGroupService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
