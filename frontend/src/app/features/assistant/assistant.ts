// assistant.service.ts
import { Injectable, signal, computed, Type, inject } from '@angular/core';
import { ProcessDefinition } from './workflow.model';
import { LoginStep } from './steps/login-step/login-step';
import { ProductSelectionStep } from './steps/product-selection-step/product-selection-step';
import { AddressSelectionStep } from './steps/address-selection-step/address-selection-step';
import { ApiService } from '../../data-access/api/api.service';

@Injectable({ providedIn: 'root' })
export class AssistantService {
  private apiService = inject(ApiService);
  private basePrompt = `You are a concise Digital Assistant. 
    Respond in 1-2 short sentences maximum. 
    Greet the user briefly and give instructions for the current step.`;

  public readonly availableProcesses: ProcessDefinition[] = [
    {
      id: 'ORDER',
      label: 'Order Product',
      icon: '📦',
      description: 'Start a new product order',
      steps: [LoginStep, ProductSelectionStep, AddressSelectionStep] // References the component class directly
    },
    {
      id: 'SUPPORT',
      label: 'Get Support',
      icon: '🎧',
      description: 'Talk to our team',
      steps: [] // Add support components here later
    }
  ];

  isOpen = signal(false);
  currentStepIndex = signal(0);
  currentWorkflow = signal<Type<any>[]>([]); // Store Class references
  collectedData = signal<AssistantState>({});

  // Computed signal for the UI to bind to
  currentComponent = computed(() => {
    const workflow = this.currentWorkflow();
    return workflow[this.currentStepIndex()] ?? null;
  });

  toggleAssistant() {
    console.log('Toggling assistant');
    this.isOpen.update(v => !v);
  }
  open() {
    this.reset(); // Clears any previous half-finished progress
    this.isOpen.set(true);
  }
  startProcess(steps: Type<any>[]) {
    this.currentWorkflow.set(steps);
    this.currentStepIndex.set(0);
    this.collectedData.set({});
  }

  // Steps call this directly
  next(data: any) {
    this.collectedData.update(v => ({ ...v, ...data }));
    this.currentStepIndex.update(i => i + 1);
  }

  reset() {
    this.currentWorkflow.set([]);
    this.currentStepIndex.set(0);
  }

  callLLM(stepContext: string, userPrompt: string = '') {
    // Determine if this is a first-time greeting or a follow-up
    const behaviorInstruction = !userPrompt
      ? "This is the start of the step. Introduce yourself and tell the user what to do."
      : "The user has responded. Acknowledge them and guide them further.";

    const fullPrompt = `
      ${this.basePrompt}
      BEHAVIOR: ${behaviorInstruction}
      CONTEXT: ${stepContext}
      ${userPrompt ? `USER SAID: ${userPrompt}` : ''}
      `.trim();
    return this.apiService.post<string>('/Ollama', { prompt: fullPrompt }, { responseType: 'text' });
  }
}

export interface AssistantState {
  address?: string;
  username?: string;
  paymentMethod?: string;
}