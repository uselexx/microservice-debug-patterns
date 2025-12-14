import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { WeatherForecast, ApiResponse } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = '/api';

  constructor(private http: HttpClient) {}

  /**
   * Get weather forecast from backend
   */
  getWeatherForecast(): Observable<WeatherForecast[]> {
    return this.http.get<WeatherForecast[]>(`${this.baseUrl}/weatherforecast`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Generic GET request
   */
  get<T>(endpoint: string): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${endpoint}`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Generic POST request
   */
  post<T>(endpoint: string, payload: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${endpoint}`, payload).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Generic PUT request
   */
  put<T>(endpoint: string, payload: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${endpoint}`, payload).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Generic DELETE request
   */
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${endpoint}`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Handle HTTP errors gracefully
   */
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = error.error.message;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    console.error('API Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
