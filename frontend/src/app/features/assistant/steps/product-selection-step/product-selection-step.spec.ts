import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductSelectionStep } from './product-selection-step';

describe('ProductSelectionStep', () => {
  let component: ProductSelectionStep;
  let fixture: ComponentFixture<ProductSelectionStep>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductSelectionStep]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductSelectionStep);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
