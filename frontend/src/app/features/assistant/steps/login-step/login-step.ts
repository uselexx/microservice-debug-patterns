import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AssistantService } from '../../assistant';

@Component({
  selector: 'app-login-step',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-step.html',
  styleUrl: './login-step.css',
})
export class LoginStep {
  private assistant = inject(AssistantService);
  username: string = '';

  submit() {
    // Directly tell the service to move to the next step
    this.assistant.next({ username: this.username });
  }
}
