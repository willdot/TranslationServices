import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { SpeechtotextService } from '../shared/services/speechtotext.service';
import { HttpClientModule } from '@angular/common/http';
import { RecordComponent } from '../shared/components/record/record.component';
import { HomeComponent } from './home/home.component';
import { WatsonComponent } from './home/results/watson/watson.component';
import { AzureComponent } from './home/results/azure/azure.component';
import { AwsComponent } from './home/results/aws/aws.component';
import { AzureTranslateComponent } from './home/results/azure-translate/azure-translate.component';
import { WatsonTranslateComponent } from './home/results/watson-translate/watson-translate.component';
import { MillisecondsToSecondsPipe } from '../shared/pipes/milliseconds-to-seconds.pipe';


@NgModule({
  imports:      [ BrowserModule, BrowserAnimationsModule, FormsModule, HttpClientModule ],
  declarations: [
    MillisecondsToSecondsPipe,
     AppComponent,
     RecordComponent,
     HomeComponent,
     WatsonComponent,
     AzureComponent,
     AwsComponent,
     AzureTranslateComponent,
     WatsonTranslateComponent
    ],
  bootstrap:    [ AppComponent ],
  providers:    [ SpeechtotextService ]
})
export class AppModule { }
