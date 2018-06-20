import { Component, Inject, ViewChild, Output, EventEmitter, OnInit, NgZone, Input } from '@angular/core';
import { AudioService } from '../../services/audio.service';
import { trigger, state, style, animate, transition, keyframes } from '@angular/animations';
import { HomeComponent } from '../../../app/home/home.component';

@Component({
  selector: 'app-record',
  templateUrl: './record.component.html',
  styleUrls: ['./record.component.css'],
  animations: [
    trigger('squishWaveForm', [
      state('in', style({transform: 'translateX(0)'})),
      transition(':enter', [
        animate('300ms ease-in', keyframes([
          style({opacity: 0, offset: 0}),
          style({opacity: 1, offset: 1}),
        ]))
      ]),
      transition(':leave', [
        animate('600ms ease-out', keyframes([
          style({opacity: 1, transform: 'translateY(0)', offset: 0}),
          style({opacity: 0, transform: 'translateY(5%)', height: '50vh', offset: 0.5}),
          style({opacity: 0, transform: 'translateY(5%)', height: '0px', offset: 1})
        ]))
      ])
    ])
  ]
})
export class RecordComponent implements OnInit {

  @ViewChild('audioElement')
  private audioElement: any;
  private wavBase64String = '';
  private recording = false;
  private completed = false;
  private visualisationValues = new Array<number>();
  private barWidth = 1;
  private barGap = 0;

  @Input() InputLanguage: String;
  @Input() OutputLangauges: Array<string>;

  @Output() audioEmitter = new EventEmitter<string>();
  @Output() completedEmitter = new EventEmitter<boolean>();

  constructor(
    private _audioService: AudioService,
    private _ngZone: NgZone
  ) { }

  ngOnInit() {
    this._audioService.visualisationCallback = this.visualisationCallback.bind(this);
  }

  visualisationCallback(this, res) {
    const i = 0;
    this._ngZone.run(() => {
      // 512 frequency channels that can be displayed (with values 1-255 UInt8)
      const divider = 2;
      this.barWidth = (100 / (res.length / divider)) - 2 * this.barGap;

      this.visualisationValues = res
      .map(j => j / 2.55) // scaling volumes between 1 and 100 (UInt8 is 1-255)
      .map(
        // the 26th element of 512 sized array representing 0 - 20khz
        // is roughly 1khz (where majority of audio will lie)
        // we take interpolated values from the array to get the
        // majority of the audio data - note 2^9 = 512
        function(value, index, arr) {
          const oneKhzIndex = Math.ceil(1000 / 20000 * res.length);
          const calculatedIndex =
            Math.pow(9 * ( (index - oneKhzIndex) / (512 - oneKhzIndex)), 2);
          const x_0 = Math.floor(calculatedIndex);
          const x_1 = Math.ceil(calculatedIndex);
          const y_0 = arr[x_0];
          const y_1 = arr[x_1];
          let linearInterpolatedVal;
          if (x_1 === x_0) {
            linearInterpolatedVal = y_0;
          } else {
            linearInterpolatedVal =
              y_0 + ( calculatedIndex - x_0 ) * ((y_1 - y_0) / (x_1 - x_0));
          }
          // If near the first / last 10% of data, smooth out:
          if (index < 0.1 * arr.length) {
            linearInterpolatedVal = Math.sin(Math.PI * (10 * index / arr.length) / 2)
             * linearInterpolatedVal;
          }
          if (index > 0.9 * arr.length) {
            linearInterpolatedVal = Math.sin(Math.PI * (10 * (arr.length - index) / arr.length) / 2)
            * linearInterpolatedVal;
          }
          return linearInterpolatedVal;
        }
      )
      .filter(
        function (value, index, Arr) {
          return index % divider === 0;
        }
      );
    });
  }

  startRecording() {
    this.wavBase64String = '';
    this._audioService.startRecording({
      video: false,
      audio: true,
      maxLength: 10,
      debug: true
    });
    this.recording = true;
  }

  stopRecording() {
    this._audioService.stopRecording();
    this._audioService.callback = this.proccessAudioStringCallback.bind(this);
    this.recording = false;
    this.wavBase64String = this._audioService.processAudioString();
  }

  proccessAudioStringCallback(this, result) {
    this.wavBase64String = result;
    this.completed = true;
    this.audioEmitter.emit(this.wavBase64String);
    this.completedEmitter.emit(this.completed);
  }

  restart() {
    const self = this;
    this._ngZone.run(() => {
      self.visualisationValues = new Array<number>();
    });
    this.completed = false;
    this.wavBase64String = '';
    this.audioEmitter.emit(this.wavBase64String);
    this.completedEmitter.emit(this.completed);
  }
}
