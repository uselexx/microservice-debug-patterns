import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmationStep } from './confirmation-step';

describe('ConfirmationStep', () => {
  let component: ConfirmationStep;
  let fixture: ComponentFixture<ConfirmationStep>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmationStep]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConfirmationStep);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
