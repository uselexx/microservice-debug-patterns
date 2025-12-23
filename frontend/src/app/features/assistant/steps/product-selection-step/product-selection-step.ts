import { Component, computed, inject, OnInit } from '@angular/core';
import { AssistantService } from '../../assistant';
import { FormsModule } from '@angular/forms';
import { catchError, Observable, of, tap } from 'rxjs';
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-product-selection-step',
  standalone: true,
  imports: [FormsModule, AsyncPipe],
  templateUrl: './product-selection-step.html',
  styleUrl: './product-selection-step.css',
})
export class ProductSelectionStep implements OnInit {
  private assistant = inject(AssistantService);

  isLoading: boolean = false;
  assistantGreeting$!: Observable<string>;
  
  previousUsername = computed(() => {
    const data = this.assistant.collectedData();
    return data.username || 'N/A';
  });

  ngOnInit(): void {
    // Any initialization logic can go here
    this.isLoading = true;

    const stepContext = 
      `The user is at the product selection step and can choose
      between various products. Greet the user by their username.`;

    const userContext = this.previousUsername();
    this.assistantGreeting$ = this.assistant.callLLM(stepContext, userContext).pipe(
      tap(() => {
        // This runs when the data arrives successfully
        this.isLoading = false;
        console.log('LLM response received, stopping animation.');
      }),
      catchError((err) => {
        console.error('Assistant Error:', err);
        this.isLoading = false;
        return of('Hello! Please enter your username to continue.');
      })
    );
  }

  submit() {
    // Directly tell the service to move to the next step
    this.assistant.next({ username: this.previousUsername() });
  }
}
