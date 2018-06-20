import { Component, OnInit, Input } from '@angular/core';
import { SpeechTranslationService } from '../../../../shared/services/speech-translation.service';

@Component({
  selector: 'app-watson-translate',
  templateUrl: './watson-translate.component.html',
  styleUrls: ['./watson-translate.component.css']
})
export class WatsonTranslateComponent implements OnInit {

  @Input() private wavBase64String = '';
  @Input() private inputLanguage = '';
  @Input() private outputLanguages = [];

  constructor(private _speechTranslationService: SpeechTranslationService) { }

  responseModel: any = null;
  error = false;

  ngOnInit() {
    if (this.outputLanguages.length === 0) {
      this.error = true;
    }

    const postObject = {
      base64String: this.wavBase64String,
      inputLanguage: this.inputLanguage,
      outputLanguages: this.outputLanguages
    };

    this._speechTranslationService.postWavWatson(postObject).subscribe(
      response => {
        console.log(response);
        if (response.jsonResult) {
          this.responseModel = response;
          this.error = false;
        } else {
          this.responseModel = null;
          this.error = true;
        }
      },
      err => {
        this.error = true;
      });
  }
}
