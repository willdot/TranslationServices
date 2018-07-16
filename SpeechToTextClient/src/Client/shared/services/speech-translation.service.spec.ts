import { TestBed, inject } from '@angular/core/testing';

import { SpeechTranslationService } from './speech-translation.service';

describe('SpeechTranslationService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SpeechTranslationService]
    });
  });

  it('should be created', inject([SpeechTranslationService], (service: SpeechTranslationService) => {
    expect(service).toBeTruthy();
  }));
});
