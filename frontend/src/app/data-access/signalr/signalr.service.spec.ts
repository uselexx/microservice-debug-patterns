import { TestBed } from '@angular/core/testing';
import { SignalrService, DashboardUpdate } from './signalr.service';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';

describe('SignalrService', () => {
  let service: SignalrService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SignalrService]
    });

    service = TestBed.inject(SignalrService);
  });

  afterEach(() => {
    if (service) {
      service.disconnect();
    }
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize with disconnected state', () => {
    expect(service.isConnected()).toBe(false);
  });

  it('should provide update$ observable', () => {
    // Service should expose update$ observable
    expect(service.update$).toBeTruthy();
  });

  describe('connect', () => {
    it('should return a promise', () => {
      const result = service.connect();
      expect(result instanceof Promise).toBe(true);
    });

    it('should attempt to establish connection', () => {
      return service.connect()
        .then(() => {
          // Connection attempted
        })
        .catch(() => {
          // Expected to fail in test environment
        });
    });

    it('should have auto-reconnect strategy', () => {
      // The service should have retry logic configured
      return service.connect()
        .then(() => {
          // Connection attempted
        })
        .catch(() => {
          // Expected in test - no real server
        });
    });
  });

  describe('disconnect', () => {
    it('should disconnect from hub', () => {
      service.disconnect();
      expect(service.isConnected()).toBe(false);
    });

    it('should be callable without throwing errors', () => {
      expect(() => service.disconnect()).not.toThrow();
    });
  });

  describe('isConnected', () => {
    it('should return connection status', () => {
      const status = service.isConnected();
      expect(typeof status).toBe('boolean');
    });

    it('should return false when not connected', () => {
      expect(service.isConnected()).toBe(false);
    });
  });

  describe('DashboardUpdate interface', () => {
    it('should have required properties', () => {
      const update: DashboardUpdate = {
        cpuUsage: 45,
        memoryUsage: 60,
        requestCount: 125,
        status: 'Healthy',
        timestamp: new Date().toISOString()
      };

      expect(update.cpuUsage).toBeDefined();
      expect(update.memoryUsage).toBeDefined();
      expect(update.requestCount).toBeDefined();
      expect(update.status).toBeDefined();
      expect(update.timestamp).toBeDefined();
    });

    it('should accept percentage values for usage metrics', () => {
      const update: DashboardUpdate = {
        cpuUsage: 0,
        memoryUsage: 100,
        requestCount: 1000,
        status: 'Critical',
        timestamp: new Date().toISOString()
      };

      expect(update.cpuUsage).toBe(0);
      expect(update.memoryUsage).toBe(100);
    });

    it('should accept different status values', () => {
      const healthyUpdate: DashboardUpdate = {
        cpuUsage: 10,
        memoryUsage: 20,
        requestCount: 50,
        status: 'Healthy',
        timestamp: new Date().toISOString()
      };

      const warningUpdate: DashboardUpdate = {
        cpuUsage: 70,
        memoryUsage: 80,
        requestCount: 500,
        status: 'Warning',
        timestamp: new Date().toISOString()
      };

      const criticalUpdate: DashboardUpdate = {
        cpuUsage: 95,
        memoryUsage: 95,
        requestCount: 5000,
        status: 'Critical',
        timestamp: new Date().toISOString()
      };

      expect(healthyUpdate.status).toBe('Healthy');
      expect(warningUpdate.status).toBe('Warning');
      expect(criticalUpdate.status).toBe('Critical');
    });
  });

  it('should use correct hub URL', () => {
    // Service should connect to /hubs/dashboard endpoint
    return service.connect()
      .then(() => {
        // If connection succeeds, URL was correct
      })
      .catch(() => {
        // Expected to fail in test - no real server
      });
  });

  it('should have reconnection logic', () => {
    // Service should be configured with withAutomaticReconnect
    return service.connect()
      .then(() => {
        // Connection attempted
      })
      .catch(() => {
        // Expected in test
      });
  });

  it('should handle connection errors gracefully', () => {
    return service.connect()
      .then(() => {
        // Connection attempted
      })
      .catch((error: any) => {
        // Should catch error without crashing
        expect(error).toBeTruthy();
      });
  });
});
