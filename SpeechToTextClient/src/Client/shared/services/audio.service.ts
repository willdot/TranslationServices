import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AudioService {

  public stream: MediaStream;
  public visualisationCallback: any = null;
  public callback = null;

  private leftchannel: any = new Array;
  private rightchannel: any = new Array;
  private recordingLength = 0;
  private bufferSize = 4096; // Effects recording speed, 4096 is a nice speed
  private sampleRate = 0;

  private processor: any = null;
  private context: AudioContext = null;
  private analyser: AnalyserNode = null;
  private analyser2: AnalyserNode = null;

  stopRecording(): any {
    const stream = this.stream;
    if (stream) {
      stream.getAudioTracks().forEach(track => track.stop());
      stream.getVideoTracks().forEach(track => track.stop());
    }

    if (this.processor) {
      this.processor.onaudioprocess = null;
      this.processor = null;
    }
    this.analyser = null;
    this.analyser2 = null;
  }

  startRecording(config) {
    const self = this;
    const browser = <any>navigator;
    browser.getUserMedia = (browser.getUserMedia ||
      browser.webkitGetUserMedia ||
      browser.mozGetUserMedia ||
      browser.msGetUserMedia);

    if (browser.mediaDevices && browser.mediaDevices.getUserMedia) {
      self.stream = null;
      self.leftchannel = new Array;
      self.rightchannel = new Array;
      self.recordingLength = 0;

      browser.mediaDevices.getUserMedia(config)
        .then(stream => {
          self.stream = stream;
          self.handleSuccess();
        })
        .catch(error => console.log('An error has occured during recording'));
    } else {
      alert('Audio capture is not supported in this browser');
    }
  }

  processAudioString() {
    if (this.stream) {
      return this.WAVProcessing();
    }
  }

  handleSuccess() {
    const stream = this.stream;
    this.context = new AudioContext();

    const sampleRate = this.context.sampleRate;
    const volume = this.context.createGain();

    const source = this.context.createMediaStreamSource(stream);
    const bufferSize = this.bufferSize;
    this.sampleRate = sampleRate;

    this.analyser = this.context.createAnalyser();
    this.analyser.smoothingTimeConstant = 0.3;
    this.analyser.fftSize = 1024;

    this.analyser2 = this.context.createAnalyser();
    this.analyser2.smoothingTimeConstant = 0.3;
    this.analyser2.fftSize = 1024;

    // create a buffer source node
    const sourceNode = this.context.createBufferSource();
    const splitter = this.context.createChannelSplitter();

    // connect the source to the analyser and the splitter
    sourceNode.connect(splitter);

    // connect one of the outputs from the splitter to
    // the analyser
    splitter.connect(this.analyser, 0, 0);
    splitter.connect(this.analyser2, 1, 0);

    source.connect(volume);
    source.connect(this.analyser);
    source.connect(this.analyser2);

    this.processor = this.context.createScriptProcessor(bufferSize, 2, 2);
    volume.connect(this.processor);
    this.analyser.connect(this.processor);
    this.processor.connect(this.context.destination);

    this.processor.onaudioprocess = this.generateSounds.bind(this, bufferSize);
  }

  generateSounds(this, bufferSize, e) {
    const left = e.inputBuffer.getChannelData(0);
    const right = e.inputBuffer.getChannelData(1);
    // we clone the samples

    this.leftchannel.push(new Float32Array(left));
    this.rightchannel.push(new Float32Array(right));
    this.recordingLength += bufferSize;

    if (this.visualisationCallback != null) {
        this.generateVisulisation();
    }
  }

  generateVisulisation() {
      const res = this.generateSpectrum();
      if (this.visualisationCallback != null) {
        this.visualisationCallback(res);
      }
  }

  generateSpectrum() {
    const array =  new Uint8Array(this.analyser.frequencyBinCount);
    this.analyser.getByteFrequencyData(array);
    return array;
  }

  generateTwoChannelVolumneData() {
      const array =  new Uint8Array(this.analyser.frequencyBinCount);
      this.analyser.getByteFrequencyData(array);
      const average = this.getAverageVolume(array);

      // get the average for the second channel
      const array2 =  new Uint8Array(this.analyser2.frequencyBinCount);
      this.analyser2.getByteFrequencyData(array2);
      const average2 = this.getAverageVolume(array2);

      return [average, average2];
  }

  getAverageVolume(array) {
    let values = 0;
    let average;

    const length = array.length;

    // get all the frequency amplitudes
    for (let i = 0; i < length; i++) {
        values += array[i];
    }

    average = values / length;
    return average;
}

  mergeBuffers(channelBuffer, recordingLength) {
    const result = new Float32Array(recordingLength);
    let offset = 0;
    const lng = channelBuffer.length;
    for (let i = 0; i < lng; i++) {
      const buffer = channelBuffer[i];
      result.set(buffer, offset);
      offset += buffer.length;
    }
    return result;
  }

  interleave(leftChannel, rightChannel) {
    const length = leftChannel.length + rightChannel.length;
    const result = new Float32Array(length);

    let inputIndex = 0;

    for (let index = 0; index < length;) {
      result[index++] = leftChannel[inputIndex];
      result[index++] = rightChannel[inputIndex];
      inputIndex++;
    }
    return result;
  }

  writeUTFBytes(view, offset, string) {
    const lng = string.length;
    for (let i = 0; i < lng; i++) {
      view.setUint8(offset + i, string.charCodeAt(i));
    }
  }

  WAVProcessing(): any {
    const leftBuffer = this.mergeBuffers(this.leftchannel, this.recordingLength);
    const rightBuffer = this.mergeBuffers(this.rightchannel, this.recordingLength);
    // we interleave both channels together
    const interleaved = this.interleave(leftBuffer, rightBuffer);

    // create the buffer and view to create the .WAV file
    const buffer = new ArrayBuffer(44 + interleaved.length * 2);
    const view = new DataView(buffer);

    // write the WAV container, check spec at: https://ccrma.stanford.edu/courses/422/projects/WaveFormat/
    // RIFF chunk descriptor
    this.writeUTFBytes(view, 0, 'RIFF');
    view.setUint32(4, 44 + interleaved.length * 2, true);
    this.writeUTFBytes(view, 8, 'WAVE');
    // FMT sub-chunk
    this.writeUTFBytes(view, 12, 'fmt ');
    view.setUint32(16, 16, true);
    view.setUint16(20, 1, true);
    // stereo (2 channels)
    view.setUint16(22, 2, true);
    view.setUint32(24, this.sampleRate, true);
    view.setUint32(28, this.sampleRate * 4, true);
    view.setUint16(32, 4, true);
    view.setUint16(34, 16, true);
    // data sub-chunk
    this.writeUTFBytes(view, 36, 'data');
    view.setUint32(40, interleaved.length * 2, true);

    // write the PCM samples
    const lng = interleaved.length;
    let index = 44;
    const volume = 1;
    for (let i = 0; i < lng; i++) {
      view.setInt16(index, interleaved[i] * (0x7FFF * volume), true);
      index += 2;
    }

    // our final binary blob that we can hand off
    const blob = new Blob([view], { type: 'audio/wav' });
    this.wavBlobToBase64(blob, 'azure');
  }

  downloadWav(blob) {
    const a = document.createElement('a');
    document.body.appendChild(a);

    const url = window.URL.createObjectURL(blob);
    a.href = url;
    a.download = 'wavwob';
    a.click();
    window.URL.revokeObjectURL(url);
  }

  wavBlobToBase64(blob: any, callbackType: any) {
    const reader = new FileReader();
    reader.readAsDataURL(blob);
    reader.onloadend = this.wavBlobToBase64Callback.bind(this);
  }

  wavBlobToBase64Callback(this, event) {
    const base64Data = event.target.result.split(',')[1];
    this.callback(base64Data);
  }
}
