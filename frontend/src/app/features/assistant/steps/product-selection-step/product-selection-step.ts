import { Component, computed, inject } from '@angular/core';
import { AssistantService } from '../../assistant';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-selection-step',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './product-selection-step.html',
  styleUrl: './product-selection-step.css',
})
export class ProductSelectionStep {
  private assistant = inject(AssistantService);
  username: string = '';

  previousUsername = computed(() => {
    const data = this.assistant.collectedData();
    return data.username || 'N/A';
  });

  submit() {
    // Directly tell the service to move to the next step
    this.assistant.next({ username: this.username });
  }
}
