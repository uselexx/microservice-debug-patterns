import { CommonModule, NgComponentOutlet } from '@angular/common';
import { Component, inject } from '@angular/core';
import { AssistantService } from '../assistant';

@Component({
  selector: 'app-digital-assistant',
  standalone: true,
  imports: [NgComponentOutlet, CommonModule],
  templateUrl: './digital-assistant.html',
  styleUrl: './digital-assistant.css',
})
export class DigitalAssistant {


  constructor(public assistant: AssistantService) { }

  handleStepComplete = (data: any) => {
    if (data) {
      this.assistant.next(data);
    }
  };



  /**
   * Optional: Logic to handle clicking outside or escape key to close
   */
  close() {
    this.assistant.isOpen.set(false);
  }
}
