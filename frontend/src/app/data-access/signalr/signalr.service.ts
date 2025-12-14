import { Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';

export interface DashboardUpdate {
  timestamp: string;
  cpuUsage: number;
  memoryUsage: number;
  requestCount: number;
  status: string;
}

@Injectable({
  providedIn: 'root'
})

export class SignalrService {
  private hubConnection: HubConnection | null = null;
  private updateSubject = new Subject<DashboardUpdate>();
  isConnected = signal(false);

  public update$ = this.updateSubject.asObservable();

  constructor() {}

  /**
   * Initialize and start SignalR connection
   */
  connect(): Promise<void> {
    return new Promise((resolve, reject) => {
      try {
        this.hubConnection = new HubConnectionBuilder()
          .withUrl('/hubs/dashboard')
          .withAutomaticReconnect([0, 0, 0, 5000, 5000, 5000])
          .configureLogging(LogLevel.Information)
          .build();

        this.hubConnection.on('ReceiveUpdate', (data: DashboardUpdate) => {
          this.updateSubject.next(data);
        });

        this.hubConnection.onreconnecting(() => {
          console.log('SignalR reconnecting...');
          this.isConnected.set(false);
        });

        this.hubConnection.onreconnected(() => {
          console.log('SignalR reconnected');
          this.isConnected.set(true);
        });

        this.hubConnection.onclose(() => {
          console.log('SignalR disconnected');
          this.isConnected.set(false);
        });

        this.hubConnection.start()
          .then(() => {
            console.log('SignalR connected');
            this.isConnected.set(true);
            resolve();
          })
          .catch(err => {
            console.error('SignalR connection error:', err);
            this.isConnected.set(false);
            reject(err);
          });
      } catch (err) {
        console.error('Error creating SignalR connection:', err);
        reject(err);
      }
    });
  }

  /**
   * Disconnect from SignalR
   */
  disconnect(): Promise<void> {
    if (!this.hubConnection) {
      return Promise.resolve();
    }
    return this.hubConnection.stop()
      .then(() => {
        console.log('SignalR disconnected');
        this.isConnected.set(false);
      })
      .catch(err => {
        console.error('Error disconnecting SignalR:', err);
      });
  }

  /**
   * Send a message to the server
   */
  sendUpdate(message: string): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('Connection not established');
    }
    return this.hubConnection.invoke('SendUpdate', message);
  }
}
