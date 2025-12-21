import { Component, signal, ChangeDetectionStrategy, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignalrService, DashboardUpdate } from '../../data-access/signalr/signalr.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AssistantService } from '../assistant/assistant';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit, OnDestroy {
  private signalrServicePrivate = inject(SignalrService);
  private destroy$ = new Subject<void>();

  protected signalrService_exposed = this.signalrServicePrivate;

  protected currentUpdate = signal<DashboardUpdate | null>(null);
  protected updateHistory = signal<DashboardUpdate[]>([]);

  constructor(public assistant: AssistantService) {}
  
  ngOnInit(): void {
    this.signalrServicePrivate.connect()
      .then(() => {
        console.log('Dashboard connected to SignalR');
      })
      .catch(err => {
        console.error('Failed to connect to SignalR:', err);
      });

    this.signalrServicePrivate.update$
      .pipe(takeUntil(this.destroy$))
      .subscribe((update: DashboardUpdate) => {
        this.currentUpdate.set(update);
        this.updateHistory.update(history => [update, ...history.slice(0, 49)]);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.signalrServicePrivate.disconnect();
  }

  get signalrService() {
    return this.signalrService_exposed;
  }
}
