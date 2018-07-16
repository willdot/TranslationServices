import { Component, OnInit, Input } from '@angular/core';
import { SpeechtotextService } from '../../../../shared/services/speechtotext.service';

@Component({
  selector: 'app-azure',
  templateUrl: './azure.component.html',
  styleUrls: ['./azure.component.css']
})
export class AzureComponent implements OnInit {
  @Input() private wavBase64String = '';

  constructor(private _speechToTextService: SpeechtotextService) { }

  responseModel: any = null;
  error = false;

  ngOnInit() {
    this._speechToTextService.postWAVAzure(this.wavBase64String).subscribe(
      response => {
        if (response.jsonResult.NBest) {
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
