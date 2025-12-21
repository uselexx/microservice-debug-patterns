import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueTypeStep } from './issue-type-step';

describe('IssueTypeStep', () => {
  let component: IssueTypeStep;
  let fixture: ComponentFixture<IssueTypeStep>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IssueTypeStep]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IssueTypeStep);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
