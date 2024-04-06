import { TestBed } from '@angular/core/testing';

import { AuthentifikationService } from './authentifikation.service';

describe('AuthentifikationService', () => {
  let service: AuthentifikationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthentifikationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
