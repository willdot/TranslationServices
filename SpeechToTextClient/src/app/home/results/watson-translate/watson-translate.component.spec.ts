import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WatsonTranslateComponent } from './watson-translate.component';

describe('WatsonTranslateComponent', () => {
  let component: WatsonTranslateComponent;
  let fixture: ComponentFixture<WatsonTranslateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WatsonTranslateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WatsonTranslateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
