import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AssistantService } from '../../assistant';
import { catchError, Observable, of, tap } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-login-step',
  standalone: true,
  imports: [FormsModule, AsyncPipe],
  templateUrl: './login-step.html',
  styleUrl: './login-step.css',
})
export class LoginStep implements OnInit {
  private assistant = inject(AssistantService);
  isloading: boolean = false;
  username: string = '';
  // Initialize it directly
  assistantGreeting$!: Observable<string>;

  ngOnInit(): void {
    // 1. Start loading state immediately
    this.isloading = true;

    // 2. Define the context for this specific step
    const stepContext = "The user is at the login screen and needs to provide their username.";

    /** * 3. Initialize the stream. 
     * We don't .subscribe() here; the 'async' pipe in the HTML will do it.
     */
    this.assistantGreeting$ = this.assistant.callLLM(stepContext).pipe(
      tap(() => {
        // This runs when the data arrives successfully
        this.isloading = false;
        console.log('LLM response received, stopping animation.');
      }),
      catchError((err) => {
        // This runs if the API fails or CORS issues occur
        console.error('Assistant Error:', err);
        this.isloading = false;
        // Return a fallback message so the UI doesn't break
        return of('Hello! Please enter your username to continue.');
      })
    );
  }

  submit() {
    // Directly tell the service to move to the next step
    this.assistant.next({ username: this.username });
  }
}
