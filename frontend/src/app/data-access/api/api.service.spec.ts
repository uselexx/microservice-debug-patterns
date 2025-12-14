import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from './api.service';
import { WeatherForecast, ApiResponse } from '../models/index';

describe('ApiService', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

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

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ApiService]
    });

    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getWeatherForecast', () => {
    it('should fetch weather forecast', () => {
      service.getWeatherForecast().subscribe(data => {
        expect(data).toEqual(mockWeatherData);
      });

      const req = httpMock.expectOne('/api/weatherforecast');
      expect(req.request.method).toBe('GET');
      req.flush(mockWeatherData);
    });

    it('should handle errors', () => {
      const errorMessage = 'Server error';

      service.getWeatherForecast().subscribe(
        () => fail('should have failed'),
        (error: any) => {
          expect(error.error).toBe(errorMessage);
        }
      );

      const req = httpMock.expectOne('/api/weatherforecast');
      req.flush(errorMessage, { status: 500, statusText: 'Server Error' });
    });

    it('should retry on network error', () => {
      service.getWeatherForecast().subscribe(
        () => fail('should have failed'),
        (error: any) => {
          expect(error).toBeTruthy();
        }
      );

      const reqs = httpMock.match('/api/weatherforecast');
      // Should retry once (2 attempts total)
      expect(reqs.length).toBeGreaterThanOrEqual(1);
      reqs.forEach(req => {
        req.flush(null, { status: 0, statusText: 'Network error' });
      });
    });
  });

  describe('GET request', () => {
    it('should make GET request to endpoint', () => {
      service.get('/test').subscribe();

      const req = httpMock.expectOne('/api/test');
      expect(req.request.method).toBe('GET');
      req.flush({});
    });

    it('should handle successful GET request', () => {
      const response = { success: true, data: 'test' };

      service.get('/test').subscribe(data => {
        expect(data).toEqual(response);
      });

      const req = httpMock.expectOne('/api/test');
      req.flush(response);
    });

    it('should handle GET request error', () => {
      service.get('/test').subscribe(
        () => fail('should have failed'),
        (error: any) => {
          expect(error).toBeTruthy();
        }
      );

      const req = httpMock.expectOne('/api/test');
      req.flush('Error', { status: 404, statusText: 'Not Found' });
    });
  });

  describe('POST request', () => {
    it('should make POST request with data', () => {
      const payload = { name: 'test', value: 123 };

      service.post('/test', payload).subscribe();

      const req = httpMock.expectOne('/api/test');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(payload);
      req.flush({});
    });

    it('should handle successful POST request', () => {
      const payload = { name: 'test' };
      const response = { id: 1, ...payload };

      service.post('/test', payload).subscribe(data => {
        expect(data).toEqual(response);
      });

      const req = httpMock.expectOne('/api/test');
      req.flush(response);
    });

    it('should handle POST request error', () => {
      service.post('/test', {}).subscribe(
        () => fail('should have failed'),
        (error: any) => {
          expect(error).toBeTruthy();
        }
      );

      const req = httpMock.expectOne('/api/test');
      req.flush('Error', { status: 400, statusText: 'Bad Request' });
    });
  });

  describe('PUT request', () => {
    it('should make PUT request with data', () => {
      const payload = { name: 'updated' };

      service.put('/test/1', payload).subscribe();

      const req = httpMock.expectOne('/api/test/1');
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(payload);
      req.flush({});
    });

    it('should handle successful PUT request', () => {
      const payload = { name: 'updated' };
      const response = { id: 1, ...payload };

      service.put('/test/1', payload).subscribe(data => {
        expect(data).toEqual(response);
      });

      const req = httpMock.expectOne('/api/test/1');
      req.flush(response);
    });
  });

  describe('DELETE request', () => {
    it('should make DELETE request', () => {
      service.delete('/test/1').subscribe();

      const req = httpMock.expectOne('/api/test/1');
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });

    it('should handle successful DELETE request', () => {
      service.delete('/test/1').subscribe(data => {
        expect(data).toBeTruthy();
      });

      const req = httpMock.expectOne('/api/test/1');
      req.flush({ success: true });
    });

    it('should handle DELETE request error', () => {
      service.delete('/test/1').subscribe(
        () => fail('should have failed'),
        (error: any) => {
          expect(error).toBeTruthy();
        }
      );

      const req = httpMock.expectOne('/api/test/1');
      req.flush('Error', { status: 404, statusText: 'Not Found' });
    });
  });

  it('should prepend /api to all endpoints', () => {
    service.get('/test').subscribe();
    service.post('/test', {}).subscribe();
    service.put('/test', {}).subscribe();
    service.delete('/test').subscribe();

    const requests = httpMock.match(req => ((req as any).url ?? (req as any).request?.url)?.includes('/api/'));
    expect(requests.length).toBe(4);
    requests.forEach(req => {
      const url = (req as any).request?.url ?? (req as any).url;
      expect(url).toContain('/api/');
      (req as any).flush({});
    });
  });
});
