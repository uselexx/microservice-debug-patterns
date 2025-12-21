import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddressSelectionStep } from './address-selection-step';

describe('AddressSelectionStep', () => {
  let component: AddressSelectionStep;
  let fixture: ComponentFixture<AddressSelectionStep>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddressSelectionStep]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddressSelectionStep);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
