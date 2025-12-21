import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DigitalAssistant } from './digital-assistant';

describe('DigitalAssistant', () => {
  let component: DigitalAssistant;
  let fixture: ComponentFixture<DigitalAssistant>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DigitalAssistant]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DigitalAssistant);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
