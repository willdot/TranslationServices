import { Component } from '@angular/core';
import { trigger, state, style, animate, transition, keyframes } from '@angular/animations';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  animations: [
    trigger('flyInOut', [
      state('in', style({transform: 'translateY(0)'})),
      transition(':enter', [
        animate('600ms ease-in', keyframes([
          style({transform: 'translateY(100%)', offset: 0}),
          style({transform: 'translateY(0)', offset: 1})
        ]))
      ]),
      transition(':leave', [
        animate('600ms ease-out', keyframes([
          style({opacity: 1, transform: 'translateY(0)', offset: 0}),
          style({opacity: 0, transform: 'translateY(100%)', offset: 1})
        ]))
      ])
    ])
  ]
})
export class AppComponent {
}
