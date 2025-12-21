import { CommonModule } from '@angular/common';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../data-access/api/api.service';
import { MoviesService } from './movies-service';

@Component({
  selector: 'app-movies',
  standalone: true,
  imports: [CommonModule, ScrollingModule],
  templateUrl: './movies.component.html',
  styleUrls: ['./movies.component.css'],
})
export class MoviesComponent implements OnInit {
  public dataSource!: MoviesService;

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    // We pass the apiService to the DataSource so it can make requests
    this.dataSource = new MoviesService(this.apiService);
  }
}
