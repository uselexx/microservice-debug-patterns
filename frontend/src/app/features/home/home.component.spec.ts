import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HomeComponent } from './home.component';
import { ApiService } from '../../data-access/api/api.service';
import { of, throwError } from 'rxjs';
import { WeatherForecast } from '../../data-access/models/index';

describe('HomeComponent', () => {
  let component: HomeComponent;
  let fixture: ComponentFixture<HomeComponent>;
  let apiService: jasmine.SpyObj<ApiService>;

  const mockWeatherData: WeatherForecast[] = [
    {
      date: '2024-01-01',
      temperatureC: 20,
      temperatureF: 68,
      summary: 'Sunny'
    },
    {
      date: '2024-01-02',
      temperatureC: 15,
      temperatureF: 59,
      summary: 'Cloudy'
    }
  ];

  beforeEach(async () => {
    const apiServiceSpy = jasmine.createSpyObj('ApiService', ['getWeatherForecast']);

    await TestBed.configureTestingModule({
      imports: [HomeComponent],
      providers: [
        { provide: ApiService, useValue: apiServiceSpy }
      ]
    }).compileComponents();

    apiService = TestBed.inject(ApiService) as jasmine.SpyObj<ApiService>;
    fixture = TestBed.createComponent(HomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render home container', () => {
    const homeContainer = fixture.nativeElement.querySelector('.home-container');
    expect(homeContainer).toBeTruthy();
  });

  it('should display welcome heading', () => {
    const heading = fixture.nativeElement.querySelector('h1');
    expect(heading?.textContent).toContain('Welcome to Microservice Debug Patterns');
  });

  it('should have fetch button', () => {
    const button = fixture.nativeElement.querySelector('button');
    expect(button?.textContent).toContain('Fetch Weather Forecast');
  });

  it('should initialize with empty weather data', () => {
    expect(component.weatherData()).toEqual([]);
  });

  it('should initialize with no loading state', () => {
    expect(component.isLoading()).toBe(false);
  });

  it('should initialize with no error message', () => {
    expect(component.errorMessage()).toBe('');
  });

  it('should call API when fetch button is clicked', () => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));
    const button = fixture.nativeElement.querySelector('button');

    button?.click();

    expect(apiService.getWeatherForecast).toHaveBeenCalled();
  });

  it('should set loading state while fetching', (done) => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));

    component.onFetchWeather();
    expect(component.isLoading()).toBe(true);

    setTimeout(() => {
      expect(component.isLoading()).toBe(false);
      done();
    }, 0);
  });

  it('should update weather data on successful fetch', (done) => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));

    component.onFetchWeather();

    setTimeout(() => {
      expect(component.weatherData()).toEqual(mockWeatherData);
      done();
    }, 0);
  });

  it('should display error message on API failure', (done) => {
    const errorMessage = 'Network error';
    apiService.getWeatherForecast.and.returnValue(throwError(() => ({ message: errorMessage })));

    component.onFetchWeather();

    setTimeout(() => {
      expect(component.errorMessage()).toContain('Error');
      expect(component.isLoading()).toBe(false);
      done();
    }, 0);
  });

  it('should display results table when data is loaded', (done) => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));

    component.onFetchWeather();

    setTimeout(() => {
      fixture.detectChanges();
      const table = fixture.nativeElement.querySelector('table');
      expect(table).toBeTruthy();

      const rows = fixture.nativeElement.querySelectorAll('tbody tr');
      expect(rows.length).toBe(mockWeatherData.length);
      done();
    }, 0);
  });

  it('should display error message in DOM', (done) => {
    const errorMsg = 'Test error message';
    apiService.getWeatherForecast.and.returnValue(throwError(() => ({ message: errorMsg })));

    component.onFetchWeather();

    setTimeout(() => {
      fixture.detectChanges();
      const errorDiv = fixture.nativeElement.querySelector('.error-message');
      expect(errorDiv).toBeTruthy();
      expect(errorDiv?.textContent).toContain('Error');
      done();
    }, 0);
  });

  it('should disable button while loading', (done) => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));

    const button = fixture.nativeElement.querySelector('button');
    button?.click();

    expect(component.isLoading()).toBe(true);
    fixture.detectChanges();
    expect(button?.disabled).toBe(true);

    setTimeout(() => {
      fixture.detectChanges();
      expect(button?.disabled).toBe(false);
      done();
    }, 0);
  });

  it('should clear previous error when fetching new data', (done) => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));

    component.errorMessage.set('Previous error');
    component.onFetchWeather();

    setTimeout(() => {
      expect(component.errorMessage()).toBe('');
      done();
    }, 0);
  });

  it('should clear previous data when fetching', (done) => {
    apiService.getWeatherForecast.and.returnValue(of(mockWeatherData));

    component.weatherData.set([{ date: 'old', temperatureC: 0, temperatureF: 32, summary: 'Old' }]);
    component.onFetchWeather();

    expect(component.weatherData()).toEqual([]);
    done();
  });
});
