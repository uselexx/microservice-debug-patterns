import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FooterComponent } from './footer.component';

describe('FooterComponent', () => {
  let component: FooterComponent;
  let fixture: ComponentFixture<FooterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FooterComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(FooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render footer element', () => {
    const footer = fixture.nativeElement.querySelector('footer');
    expect(footer).toBeTruthy();
  });

  it('should display copyright text', () => {
    const footerText = fixture.nativeElement.textContent;
    expect(footerText).toContain('©');
    expect(footerText).toContain('Microservice Debug Patterns');
  });

  it('should display current year', () => {
    const currentYear = new Date().getFullYear();
    const footerText = fixture.nativeElement.textContent;
    expect(footerText).toContain(currentYear.toString());
  });
});
