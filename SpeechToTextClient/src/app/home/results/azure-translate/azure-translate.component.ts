import { Component, OnInit, Input } from '@angular/core';
import { SpeechTranslationService } from '../../../../shared/services/speech-translation.service';

@Component({
  selector: 'app-azure-translate',
  templateUrl: './azure-translate.component.html',
  styleUrls: ['./azure-translate.component.css']
})
export class AzureTranslateComponent implements OnInit {

  @Input() private wavBase64String = '';
  @Input() private inputLanguage = '';
  @Input() private outputLanguages = [];
  constructor(private _speechTranslationSevice: SpeechTranslationService) { }

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

    this._speechTranslationSevice.postWavAzure(postObject).subscribe(
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
