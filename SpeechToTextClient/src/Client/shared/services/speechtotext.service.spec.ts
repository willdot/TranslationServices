import { TestBed, inject } from '@angular/core/testing';

import { SpeechtotextService } from './speechtotext.service';

describe('SpeechtotextService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SpeechtotextService]
    });
  });

  it('should be created', inject([SpeechtotextService], (service: SpeechtotextService) => {
    expect(service).toBeTruthy();
  }));
});
