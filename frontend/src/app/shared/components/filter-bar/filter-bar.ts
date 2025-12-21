import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-filter-bar',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './filter-bar.html',
  styleUrl: './filter-bar.css',
})
export class FilterBar implements OnInit {
  @Input() config: FilterConfig[] = [];
  @Output() onFilterChange = new EventEmitter<any>();

  filterForm = new FormGroup({});

  ngOnInit() {
    // Dynamically build the form based on config
    this.config.forEach(control => {
      this.filterForm.addControl(control.key, new FormControl(''));
    });

    // Emit changes (debounced for text fields)
    this.filterForm.valueChanges.subscribe(val => {
      this.onFilterChange.emit(val);
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