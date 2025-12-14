export interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

export interface ApiResponse<T> {
  data?: T;
  error?: string;
  message?: string;
}
