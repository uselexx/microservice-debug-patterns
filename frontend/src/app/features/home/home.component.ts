import { Component, signal, ChangeDetectionStrategy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../data-access/api/api.service';
import { WeatherForecast } from '../../data-access/models/index';
import { FilterBar, FilterConfig } from '../../shared/components/filter-bar/filter-bar';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FilterBar],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent implements OnInit {
  weatherData = signal<WeatherForecast[]>([]);
  isLoading = signal(false);
  errorMessage = signal('');
  filterConfig = signal<FilterConfig[]>([]);
  private apiService = inject(ApiService);

  ngOnInit(): void {
    this.loadFilterMetadata();
  }

  loadFilterMetadata(): void {
    this.isLoading.set(true);
    this.apiService.get<any>('/movies/filters').subscribe({
      next: (data) => {
        // Set the config signal with the API response
        this.filterConfig.set([
          { key: 'search', label: 'Keyword', type: 'text' },
          { key: 'category', label: 'Category', type: 'select', options: data.categories },
          { key: 'tags', label: 'Tags', type: 'multiselect', options: data.tags }
        ]);
        this.isLoading.set(false);
      },
      error: (err) => console.error('Error loading filter data:', err)
    });
  }

  handleFilterChange(filters: any): void {
    console.log('Filters changed:', filters);
  }

  onFetchWeather(): void {
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

