import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NavbarComponent } from './navbar.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NavbarComponent, RouterTestingModule]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render navbar container', () => {
    const navbar = fixture.nativeElement.querySelector('nav');
    expect(navbar).toBeTruthy();
  });

  it('should render app title', () => {
    const title = fixture.nativeElement.querySelector('a.navbar-brand');
    expect(title?.textContent).toContain('Microservice Debug Patterns');
  });

  it('should have Home and Dashboard links', () => {
    const links = fixture.nativeElement.querySelectorAll('a[routerLink]');
    expect(links.length).toBeGreaterThanOrEqual(2);
  });

  it('should have Home link pointing to /', () => {
    const homeLink = Array.from(fixture.nativeElement.querySelectorAll('a[routerLink]'))
      .find((link: any) => link.textContent?.includes('Home')) as HTMLElement;
    expect((homeLink as any)?.getAttribute('routerLink')).toBe('/');
  });

  it('should have Dashboard link pointing to /dashboard', () => {
    const dashboardLink = Array.from(fixture.nativeElement.querySelectorAll('a[routerLink]'))
      .find((link: any) => link.textContent?.includes('Dashboard')) as HTMLElement;
    expect((dashboardLink as any)?.getAttribute('routerLink')).toBe('/dashboard');
  });
});
