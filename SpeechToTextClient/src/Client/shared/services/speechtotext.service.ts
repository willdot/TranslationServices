import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable} from 'rxjs';

import 'rxjs/add/observable/throw';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';


@Injectable({
  providedIn: 'root'
})
export class SpeechtotextService {

  _apiRoot = 'http://localhost:51724/api/speechToText/';
  constructor(private _http: HttpClient) { }

  postWAVAzure(wavBlob: any): Observable<any> {

    const body = {base64String : wavBlob};
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

  postWAVWatson(wavBlob: any): Observable<any> {

    const body = JSON.stringify({Base64String : wavBlob});
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

  postWAVAWS(wavBlob: any): Observable<any> {

    const body = JSON.stringify({Base64String : wavBlob});
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const options = {headers: headers};

    const t0 = performance.now();
    let t1 = null;

    return this._http.post<any>(this._apiRoot + 'parseAWS', body, options)
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
