import { Component, OnInit } from '@angular/core';
import { trigger, state, style, animate, transition, keyframes } from '@angular/animations';
import { Languages } from '../../shared/models/languages';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  animations: [
    trigger('flyInOutWithFade', [
      state('in', style({ transform: 'translateY(0)' })),
      transition(':enter', [
        animate('600ms ease-in', keyframes([
          style({ opacity: 0, height: '0px', transform: 'translateY(-100%)', offset: 0 }),
          style({ opacity: 0, height: 'unset', transform: 'translateY(-100%)', offset: 0.5 }),
          style({ opacity: 1, transform: 'translateY(0)', offset: 1 })
        ]))
      ]),
      transition(':leave', [
        animate('300ms ease-out', keyframes([
          style({ opacity: 1, transform: 'translateY(0)', offset: 0 }),
          style({ opacity: 0, transform: 'translateY(5%)', offset: 1 })
        ]))
      ])
    ]),
    trigger('flyInOutTop', [
      state('in', style({ transform: 'translateY(0)' })),
      transition(':enter', [
        animate('600ms ease-in', keyframes([
          style({ transform: 'translateY(-100%)', offset: 0 }),
          style({ transform: 'translateY(0)', offset: 1 })
        ]))
      ]),
      transition(':leave', [
        animate('600ms ease-out', keyframes([
          style({ opacity: 1, transform: 'translateY(0)', offset: 0 }),
          style({ opacity: 0, transform: 'translateY(-100%)', offset: 1 })
        ]))
      ])
    ]),
    trigger('slideInOutBottom', [
      state('in', style({ transform: 'translateY(0)' })),
      transition(':enter', [
        animate('600ms ease-in', keyframes([
          style({ opacity: 0, transform: 'translateY(5%)', offset: 0 }),
          style({ opacity: 1, transform: 'translateY(0)', offset: 1 })
        ]))
      ]),
      transition(':leave', [
        animate('600ms ease-out', keyframes([
          style({ opacity: 1, transform: 'translateY(0)', offset: 0 }),
          style({ opacity: 0, transform: 'translateY(5%)', offset: 1 })
        ]))
      ])
    ])
  ]
})
export class HomeComponent {
   title = 'Speech to Text Comparator';
   wavBase64String = '';
  private completed = false;
   translations = false;
   type = 'Translations';
  private inputLanguageCode = 'en-GB';
  private outputLanguageCodes = [];

  private languages: Languages[] = [
    {
      code: 'en-GB',
      name: 'English'
    },
    {
      code: 'fr-FR',
      name: 'French'
    },
    {
      code: 'es-ES',
      name: 'Spanish'
    }
  ];

  handleAudioChange(wavBase64String: string) {
    this.wavBase64String = wavBase64String;
  }

  handleCompletion(completed: boolean) {
    this.completed = completed;
  }

  onEnableTranslationsClick(): void {
    this.translations = !this.translations;

    this.wavBase64String = '';

    if (this.translations) {
      this.type = 'Speech to text';
    } else {
      this.type = 'Translations';
    }
  }

  languageChanged(newValue: any) {
    this.inputLanguageCode = newValue;
  }

  onOutputChange(langaugeCode: string, isChecked: boolean) {

    if (isChecked) {
      this.outputLanguageCodes.push(langaugeCode);
    } else {
      const index = this.outputLanguageCodes.indexOf(langaugeCode);
      this.outputLanguageCodes.splice(index, 1);
    }
  }
}
