import { Component, OnInit, Input } from '@angular/core';
import { SpeechtotextService } from '../../../../shared/services/speechtotext.service';

@Component({
  selector: 'app-aws',
  templateUrl: './aws.component.html',
  styleUrls: ['./aws.component.css']
})
export class AwsComponent implements OnInit {
  @Input() private wavBase64String = '';

  constructor(private _speechToTextService: SpeechtotextService) { }

  responseModel: any = null;
  error = false;

  ngOnInit() {

    this._speechToTextService.postWAVAWS(this.wavBase64String).subscribe(
      response => {
        if (response.jsonResult.results.transcripts[0].transcript != ""){
          this.responseModel = response;
          this.error=false;
        }else{
          this.responseModel = null;
          this.error = true;
        }
      },
      err => {
        this.error = true;
      })

  }

}
