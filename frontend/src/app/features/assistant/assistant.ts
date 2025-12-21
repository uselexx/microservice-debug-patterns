// assistant.service.ts
import { Injectable, signal, computed, Type } from '@angular/core';
import { ProcessDefinition } from './workflow.model';
import { LoginStep } from './steps/login-step/login-step';
import { ProductSelectionStep } from './steps/product-selection-step/product-selection-step';
import { AddressSelectionStep } from './steps/address-selection-step/address-selection-step';

@Injectable({ providedIn: 'root' })
export class AssistantService {
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
  collectedData = signal<Record<string, any>>({});

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
}