import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AzureTranslateComponent } from './azure-translate.component';

describe('AzureTranslateComponent', () => {
  let component: AzureTranslateComponent;
  let fixture: ComponentFixture<AzureTranslateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AzureTranslateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AzureTranslateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
