import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AssistantService } from '../../assistant';

@Component({
  selector: 'app-login-step',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-step.html',
  styleUrl: './login-step.css',
})
export class LoginStep implements OnInit {
  private assistant = inject(AssistantService);
  private cdr = inject(ChangeDetectorRef); // 2. Inject it
  isloading: boolean = false;
  username: string = '';
  assistantGreeting: string = 'Hello! Please enter your username to continue.';

  ngOnInit(): void {
    this.isloading = true;
    this.cdr.detectChanges(); // Force dots to show immediately
    this.assistantGreeting = '';
    const stepContext = "The user is at the login screen and needs to provide their username.";

    this.assistant.callLLM(stepContext).subscribe({
      next: (response: string) => {
        this.assistantGreeting = response;
        this.isloading = false; // This must trigger the @else block
        this.cdr.detectChanges();
        console.log('Loading set to false');
      },
      error: (err) => {
        this.assistantGreeting = 'Error: Unable to connect.';
        this.isloading = false;
        this.cdr.detectChanges();
      }
    });
  }

  submit() {
    // Directly tell the service to move to the next step
    this.assistant.next({ username: this.username });
  }
}
