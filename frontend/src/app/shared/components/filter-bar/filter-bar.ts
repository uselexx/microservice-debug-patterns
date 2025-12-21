import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter-bar',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './filter-bar.html',
  styleUrl: './filter-bar.css',
})
export class FilterBar implements OnInit, OnChanges {
  @Input() config: FilterConfig[] = [];
  @Input() isLoading = false; // New input for loading state
  @Output() onFilterChange = new EventEmitter<any>();

  filterForm = new FormGroup({});

  ngOnInit() {
    this.buildForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    // When config updates from the API, rebuild the form controls
    if (changes['config'] && !changes['config'].firstChange) {
      this.buildForm();
    }
  }

  private buildForm() {
    this.config.forEach(control => {
      if (!this.filterForm.contains(control.key)) {
        this.filterForm.addControl(control.key, new FormControl(''));
      }
    });
  }

  // Inside FilterBarComponent
  get activeFilterCount(): number {
    const values = this.filterForm.value;
    return Object.values(values).filter(v => v !== null && v !== '' && (Array.isArray(v) ? v.length > 0 : true)).length;
  }

  resetFilters() {
    this.filterForm.reset();
  }
}

export interface FilterOption {
  value: any;
  label: string;
}

export interface FilterConfig {
  key: string;
  label: string;
  type: 'text' | 'select' | 'multiselect';
  options?: FilterOption[]; // Use the interface here
}