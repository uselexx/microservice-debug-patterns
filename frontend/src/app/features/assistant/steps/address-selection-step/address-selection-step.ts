import { Component, inject } from '@angular/core';
import { AssistantService } from '../../assistant';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-address-selection-step',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './address-selection-step.html',
  styleUrl: './address-selection-step.css',
})
export class AddressSelectionStep {
  private assistant = inject(AssistantService);
  address = '';

  submit() {
    // Directly tell the service to move to the next step
    this.assistant.next({ address: this.address });
  }
}
