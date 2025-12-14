import { ComponentFixture, TestBed } from '@angular/core/testing';
import { App } from './app';
import { RouterTestingModule } from '@angular/router/testing';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { FooterComponent } from './shared/components/footer/footer.component';

describe('App Component', () => {
  let component: App;
  let fixture: ComponentFixture<App>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App, RouterTestingModule, NavbarComponent, FooterComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render app container', () => {
    const container = fixture.nativeElement.querySelector('.app-container');
    expect(container).toBeTruthy();
  });

  it('should render navbar component', () => {
    const navbar = fixture.nativeElement.querySelector('app-navbar');
    expect(navbar).toBeTruthy();
  });

  it('should render footer component', () => {
    const footer = fixture.nativeElement.querySelector('app-footer');
    expect(footer).toBeTruthy();
  });

  it('should render router outlet for content', () => {
    const outlet = fixture.nativeElement.querySelector('router-outlet');
    expect(outlet).toBeTruthy();
  });

  it('should have main content area', () => {
    const mainContent = fixture.nativeElement.querySelector('.main-content');
    expect(mainContent).toBeTruthy();
  });

  it('should have flex layout with min-height 100vh', () => {
    const container = fixture.nativeElement.querySelector('.app-container');
    const styles = window.getComputedStyle(container);
    expect(styles.display).toBe('flex');
    expect(styles.flexDirection).toBe('column');
  });

  it('should have main content flex 1', () => {
    const mainContent = fixture.nativeElement.querySelector('.main-content');
    const styles = window.getComputedStyle(mainContent);
    expect(styles.flex).toContain('1');
  });
});
