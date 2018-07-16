import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable} from 'rxjs';

import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';
import { TranslationPost } from '../models/translation-post';
@Injectable({
  providedIn: 'root'
})
export class SpeechTranslationService {

  _apiRoot = 'https://translationservices.azurewebsites.net/api/SpeechTranslation/';
  constructor(private _http: HttpClient) { }

  postWavAzure(wavBlob: TranslationPost): Observable<any> {

    const body = JSON.stringify(wavBlob);
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = {headers: headers};

    const t0 = performance.now();
    let t1 = null;

    return this._http.post<any>(this._apiRoot + 'parseAzure', body, options)
      .do(data => {
        t1 = performance.now();
      })
      .map(data => {
        return this.processRecogntionModel(data, t1 - t0);
      })
      .catch(this.handleError);
  }

  postWavWatson(wavBlob: TranslationPost): Observable<any> {

    const body = JSON.stringify(wavBlob);
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = {headers: headers};

    const t0 = performance.now();
    let t1 = null;

    return this._http.post<any>(this._apiRoot + 'parseWatson', body, options)
      .do(data => {
        t1 = performance.now();
      })
      .map(data => {
        return this.processRecogntionModel(data, t1 - t0);
      })
      .catch(this.handleError);
  }


  processRecogntionModel(data, time) {
    const res = {
        detectedText : data.detectedText,
        statusCode : data.statusCode,
        jsonResult : JSON.parse(data.jsonResult),
        externalServiceTimeInMilliseconds : data.externalServiceTimeInMilliseconds,
        totalBackendTimeInMilliseconds : data.totalBackendTimeInMilliseconds,
        totalHttpTimeInMilliseconds : time
    };

    return res;
  }

  private handleError(err: HttpErrorResponse) {
    console.log(err.message);
    return Observable.throw(err.message);
  }

}
