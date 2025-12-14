import { Component, signal, ChangeDetectionStrategy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../data-access/api/api.service';
import { WeatherForecast } from '../../data-access/models/index';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {
  weatherData = signal<WeatherForecast[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');

  private apiService = inject(ApiService);

  ngOnInit(): void {
    // Optional: Load data on init
  }

  onFetchWeather(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.weatherData.set([]);

    this.apiService.getWeatherForecast().subscribe({
      next: (data: WeatherForecast[]) => {
        this.weatherData.set(data);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(`Error: ${err.message || 'Failed to fetch data from backend'}`);
        this.isLoading.set(false);
      }
    });
  }
}

