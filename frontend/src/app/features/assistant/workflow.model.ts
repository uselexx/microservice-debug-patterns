// workflow.models.ts
export interface StepConfig {
  component: any; // The Angular Component class
  label: string;
  data?: any;
}

export interface ProcessDefinition {
  id: string;
  label: string;
  description: string;
  icon: string;
  steps: any[]; // Array of component classes
}

export type WorkflowType = 'ORDER' | 'SUPPORT';