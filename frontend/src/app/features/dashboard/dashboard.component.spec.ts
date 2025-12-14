import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { SignalrService, DashboardUpdate } from '../../data-access/signalr/signalr.service';
import { of, Subject } from 'rxjs';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  // avoid referencing the `jasmine` namespace in a type to prevent TS type errors in editors
  let signalrService: any;
  let updateSubject: Subject<DashboardUpdate>;

  const mockUpdate: DashboardUpdate = {
    cpuUsage: 45,
    memoryUsage: 60,
    requestCount: 125,
    status: 'Healthy',
    timestamp: new Date().toISOString()
  };

  beforeEach(async () => {
    updateSubject = new Subject<DashboardUpdate>();
    const signalrServiceSpy = jasmine.createSpyObj(
      'SignalrService',
      ['connect', 'disconnect', 'isConnected'],
      { update$: updateSubject.asObservable() }
    );

    signalrServiceSpy.connect.and.returnValue(Promise.resolve());
    signalrServiceSpy.isConnected.and.returnValue(false);

    await TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: SignalrService, useValue: signalrServiceSpy }
      ]
    }).compileComponents();

    signalrService = TestBed.inject(SignalrService) as any;
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render dashboard container', () => {
    const container = fixture.nativeElement.querySelector('.dashboard-container');
    expect(container).toBeTruthy();
  });

  it('should display connection status', () => {
    const status = fixture.nativeElement.querySelector('.connection-status');
    expect(status).toBeTruthy();
  });

  it('should call connect on SignalR service on init', (done) => {
    component.ngOnInit();
    setTimeout(() => {
      expect(signalrService.connect).toHaveBeenCalled();
      done();
    }, 0);
  });

  it('should initialize with null current update', () => {
    expect((component as any).currentUpdate()).toBeNull();
  });

  it('should initialize with empty update history', () => {
    expect((component as any).updateHistory()).toEqual([]);
  });

  it('should update current update when receiving message', (done) => {
    component.ngOnInit();

    updateSubject.next(mockUpdate);

    setTimeout(() => {
      expect((component as any).currentUpdate()).toEqual(mockUpdate);
      done();
    }, 0);
  });

  it('should add update to history when receiving message', (done) => {
    component.ngOnInit();

    updateSubject.next(mockUpdate);

    setTimeout(() => {
      const history = (component as any).updateHistory();
      expect(history.length).toBe(1);
      expect(history[0]).toEqual(mockUpdate);
      done();
    }, 0);
  });

  it('should add new updates at beginning of history (FIFO)', (done) => {
    component.ngOnInit();

    const update1 = { ...mockUpdate, requestCount: 100 };
    const update2 = { ...mockUpdate, requestCount: 200 };

    updateSubject.next(update1);
    setTimeout(() => {
      updateSubject.next(update2);

      setTimeout(() => {
        const history = (component as any).updateHistory();
        expect(history[0]).toEqual(update2);
        expect(history[1]).toEqual(update1);
        done();
      }, 0);
    }, 0);
  });

  it('should limit history to 50 items', (done) => {
    component.ngOnInit();

    for (let i = 0; i < 60; i++) {
      const update = { ...mockUpdate, requestCount: i };
      updateSubject.next(update);
    }

    setTimeout(() => {
      expect((component as any).updateHistory().length).toBeLessThanOrEqual(50);
      done();
    }, 0);
  });

  it('should display metric cards', () => {
    const cards = fixture.nativeElement.querySelectorAll('.metric-card');
    expect(cards.length).toBeGreaterThanOrEqual(4);
  });

  it('should display CPU usage metric', () => {
    (component as any).currentUpdate.set(mockUpdate);
    fixture.detectChanges();

    const cpuText = fixture.nativeElement.textContent;
    expect(cpuText).toContain('CPU Usage');
    expect(cpuText).toContain('45%');
  });

  it('should display memory usage metric', () => {
    (component as any).currentUpdate.set(mockUpdate);
    fixture.detectChanges();

    const memoryText = fixture.nativeElement.textContent;
    expect(memoryText).toContain('Memory Usage');
    expect(memoryText).toContain('60%');
  });

  it('should display request count metric', () => {
    (component as any).currentUpdate.set(mockUpdate);
    fixture.detectChanges();

    const requestText = fixture.nativeElement.textContent;
    expect(requestText).toContain('Request Count');
    expect(requestText).toContain('125');
  });

  it('should display status metric', () => {
    (component as any).currentUpdate.set(mockUpdate);
    fixture.detectChanges();

    const statusText = fixture.nativeElement.textContent;
    expect(statusText).toContain('Status');
    expect(statusText).toContain('Healthy');
  });

  it('should display update history log', () => {
    (component as any).currentUpdate.set(mockUpdate);
    (component as any).updateHistory.set([mockUpdate]);
    fixture.detectChanges();

    const log = fixture.nativeElement.querySelector('.updates-log');
    expect(log).toBeTruthy();
  });

  it('should call disconnect on destroy', (done) => {
    component.ngOnDestroy();
    setTimeout(() => {
      expect(signalrService.disconnect).toHaveBeenCalled();
      done();
    }, 0);
  });

  it('should unsubscribe from updates on destroy', (done) => {
    component.ngOnInit();
    updateSubject.next(mockUpdate);

    component.ngOnDestroy();

    setTimeout(() => {
      const beforeLength = (component as any).updateHistory().length;
      updateSubject.next({ ...mockUpdate, requestCount: 999 });

        setTimeout(() => {
        expect((component as any).updateHistory().length).toBe(beforeLength);
        done();
      }, 0);
    }, 0);
  });

  it('should display progress bars for CPU and memory', () => {
    (component as any).currentUpdate.set(mockUpdate);
    fixture.detectChanges();

    const barFills = fixture.nativeElement.querySelectorAll('.bar-fill');
    expect(barFills.length).toBeGreaterThanOrEqual(2);
  });

  it('should update progress bar width based on CPU usage', () => {
    (component as any).currentUpdate.set({ ...mockUpdate, cpuUsage: 75 });
    fixture.detectChanges();

    const barFill = fixture.nativeElement.querySelector('.bar-fill');
    expect(barFill?.style.width).toBe('75%');
  });

  it('should display default values when no update received', () => {
    fixture.detectChanges();

    const metricsText = fixture.nativeElement.textContent;
    expect(metricsText).toContain('0%');
  });

  it('should show disconnected status when not connected', () => {
    signalrService.isConnected.and.returnValue(false);
    fixture.detectChanges();

    const status = fixture.nativeElement.querySelector('.connection-status');
    expect(status?.textContent).toContain('Disconnected');
  });

  it('should show connected status when connected', () => {
    signalrService.isConnected.and.returnValue(true);
    fixture.detectChanges();

    const status = fixture.nativeElement.querySelector('.connection-status');
    expect(status?.textContent).toContain('Connected');
  });
});
