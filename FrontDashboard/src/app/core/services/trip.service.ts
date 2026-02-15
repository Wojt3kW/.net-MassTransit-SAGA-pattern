import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, interval, switchMap, startWith, catchError, of } from 'rxjs';
import {
  TripBooking,
  SagaState,
  CreateTripRequest,
  CancelTripRequest,
  RefundTripRequest,
  FlightReservation,
  HotelReservation,
  PaymentTransaction,
  PagedSagaResponse,
  PagedTripsResponse
} from '../models/trip.models';

@Injectable({
  providedIn: 'root'
})
export class TripService {
  private readonly tripApiUrl = 'https://localhost:7172/api';
  private readonly sagaApiUrl = 'https://localhost:7276/api';
  private readonly flightApiUrl = 'https://localhost:7246/api';
  private readonly hotelApiUrl = 'https://localhost:7081/api';
  private readonly paymentApiUrl = 'https://localhost:7123/api';

  // Reactive state
  readonly trips = signal<TripBooking[]>([]);
  readonly sagaStates = signal<SagaState[]>([]);
  readonly selectedTrip = signal<TripBooking | null>(null);
  readonly selectedSaga = signal<SagaState | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // Computed - based on sagaStates for real-time SSE updates
  readonly sagaCount = computed(() => this.sagaStates().length);
  readonly sagaCompletedCount = computed(() => this.sagaStates().filter(s => s.currentState === 'Completed').length);
  readonly sagaRefundedCount = computed(() => this.sagaStates().filter(s => s.currentState === 'Refunded').length);
  readonly sagaFailedCount = computed(() => this.sagaStates().filter(s => s.currentState === 'Failed').length);
  readonly sagaCancelledCount = computed(() => this.sagaStates().filter(s => s.currentState === 'Cancelled').length);
  readonly sagaTimedOutCount = computed(() => this.sagaStates().filter(s => s.currentState === 'TimedOut').length);
  readonly sagaInProgressCount = computed(() => this.sagaStates().filter(s =>
    !['Completed', 'Refunded', 'Failed', 'Cancelled', 'TimedOut'].includes(s.currentState)
  ).length);

  // Legacy computed based on trips (for backwards compatibility)
  readonly tripCount = computed(() => this.trips().length);
  readonly completedCount = computed(() => this.trips().filter(t => t.status === 'Completed').length);
  readonly failedCount = computed(() => this.trips().filter(t => t.status === 'Failed').length);
  readonly cancelledCount = computed(() => this.trips().filter(t => t.status === 'Cancelled').length);
  readonly pendingCount = computed(() => this.trips().filter(t => t.status === 'Pending').length);

  constructor(private http: HttpClient) {}

  // === Trip API ===

  getTrips(page: number = 1, pageSize: number = 100, state?: string, customerId?: string): Observable<PagedTripsResponse> {
    let url = `${this.tripApiUrl}/trips?page=${page}&pageSize=${pageSize}`;
    if (state) url += `&state=${encodeURIComponent(state)}`;
    if (customerId) url += `&customerId=${encodeURIComponent(customerId)}`;
    return this.http.get<PagedTripsResponse>(url);
  }

  getTrip(tripId: string): Observable<TripBooking> {
    return this.http.get<TripBooking>(`${this.tripApiUrl}/trips/${tripId}`);
  }

  createTrip(request: CreateTripRequest): Observable<TripBooking> {
    return this.http.post<TripBooking>(`${this.tripApiUrl}/trips`, request);
  }

  cancelTrip(tripId: string, reason: string): Observable<void> {
    const request: CancelTripRequest = { reason };
    return this.http.post<void>(`${this.tripApiUrl}/trips/${tripId}/cancel`, request);
  }

  refundTrip(tripId: string, reason: string): Observable<void> {
    const request: RefundTripRequest = { reason };
    return this.http.post<void>(`${this.tripApiUrl}/trips/${tripId}/refund`, request);
  }

  // === SAGA API ===

  getSagaStates(page: number = 1, pageSize: number = 100, state?: string, customerId?: string) : Observable<PagedSagaResponse> {
    let url = `${this.sagaApiUrl}/sagas?page=${page}&pageSize=${pageSize}`;
    if (state) url += `&state=${encodeURIComponent(state)}`;
    if (customerId) url += `&customerId=${encodeURIComponent(customerId)}`;
    return this.http.get<PagedSagaResponse>(url);
  }

  getSagaState(tripId: string) : Observable<SagaState> {
    return this.http.get<SagaState>(`${this.sagaApiUrl}/sagas/${tripId}`);
  }

  // === Flight API ===

  getFlightReservations(tripId?: string): Observable<FlightReservation[]> {
    const url = tripId
      ? `${this.flightApiUrl}/flight-reservations?tripId=${tripId}`
      : `${this.flightApiUrl}/flight-reservations`;
    return this.http.get<FlightReservation[]>(url);
  }

  getFlightsByTripId(tripId: string): Observable<{ items: FlightReservation[] }> {
    return this.http.get<{ items: FlightReservation[] }>(`${this.flightApiUrl}/flight-reservations?tripId=${tripId}`);
  }

  // === Hotel API ===

  getHotelReservations(tripId?: string): Observable<HotelReservation[]> {
    const url = tripId
      ? `${this.hotelApiUrl}/hotel-reservations?tripId=${tripId}`
      : `${this.hotelApiUrl}/hotel-reservations`;
    return this.http.get<HotelReservation[]>(url);
  }

  getHotelsByTripId(tripId: string): Observable<{ items: HotelReservation[] }> {
    return this.http.get<{ items: HotelReservation[] }>(`${this.hotelApiUrl}/hotel-reservations?tripId=${tripId}`);
  }

  // === Payment API ===

  getPaymentTransactions(tripId?: string): Observable<PaymentTransaction[]> {
    const url = tripId
      ? `${this.paymentApiUrl}/payment-transactions?tripId=${tripId}`
      : `${this.paymentApiUrl}/payment-transactions`;
    return this.http.get<PaymentTransaction[]>(url);
  }

  getPaymentsByTripId(tripId: string): Observable<{ items: PaymentTransaction[] }> {
    return this.http.get<{ items: PaymentTransaction[] }>(`${this.paymentApiUrl}/payment-transactions?tripId=${tripId}`);
  }

  // === Reactive Data Loading ===

  loadTrips(): void {
    this.loading.set(true);
    this.getTrips().pipe(
      catchError(err => {
        this.error.set('Failed to load trips: ' + err.message);
        return of({ items: [] });
      })
    ).subscribe(response => {
      this.trips.set(response.items ?? []);
      this.loading.set(false);
    });
  }

  loadSagaStates(): void {
    this.getSagaStates().pipe(
      catchError(err => {
        this.error.set('Failed to load saga states: ' + err.message);
        return of({ items: [] });
      })
    ).subscribe(states => {
      this.sagaStates.set(states.items);
    });
  }

  loadTripDetails(tripId: string): void {
    this.loading.set(true);
    this.getTrip(tripId).pipe(
      catchError(err => {
        this.error.set('Failed to load trip: ' + err.message);
        return of(null);
      })
    ).subscribe(trip => {
      this.selectedTrip.set(trip);
      this.loading.set(false);
    });

    this.getSagaState(tripId).pipe(
      catchError(() => of(null))
    ).subscribe(saga => {
      this.selectedSaga.set(saga);
    });
  }

  // Auto-refresh with SSE (Server-Sent Events)
  private sagaEventSource?: EventSource;
  private sseBuffer: SagaState[] = [];
  private sseFlushTimeout?: ReturnType<typeof setTimeout>;

  startSagaSSE(): Observable<SagaState[]> {
    return new Observable<SagaState[]>(observer => {
      // Close existing connection if any
      this.stopSagaSSE();
      this.sseBuffer = [];

      this.sagaEventSource = new EventSource(`${this.sagaApiUrl}/sagas/stream`);

      this.sagaEventSource.onmessage = (event) => {
        try {
          const saga: SagaState = JSON.parse(event.data);
          this.sseBuffer.push(saga);

          // Debounce: wait for all sagas to arrive before updating UI
          if (this.sseFlushTimeout) {
            clearTimeout(this.sseFlushTimeout);
          }
          this.sseFlushTimeout = setTimeout(() => {
            if (this.sseBuffer.length > 0) {
              this.sagaStates.set([...this.sseBuffer]);
              observer.next([...this.sseBuffer]);
              this.sseBuffer = [];
            }
          }, 100);
        } catch (e) {
          console.error('Failed to parse SSE data:', e);
        }
      };

      this.sagaEventSource.onerror = (error) => {
        console.error('SSE connection error:', error);
        this.error.set('SSE connection lost. Reconnecting...');
        // EventSource auto-reconnects, no need to handle manually
      };

      this.sagaEventSource.onopen = () => {
        console.log('SSE connection established');
        this.error.set(null);
      };

      // Cleanup on unsubscribe
      return () => {
        this.stopSagaSSE();
      };
    });
  }

  stopSagaSSE(): void {
    if (this.sseFlushTimeout) {
      clearTimeout(this.sseFlushTimeout);
      this.sseFlushTimeout = undefined;
    }
    if (this.sagaEventSource) {
      this.sagaEventSource.close();
      this.sagaEventSource = undefined;
    }
  }

  // Legacy polling (fallback if SSE not supported)
  startPolling(intervalMs: number = 2000): Observable<{ trips: TripBooking[], sagas: SagaState[] }> {
    return interval(intervalMs).pipe(
      startWith(0),
      switchMap(() => {
        return new Observable<{ trips: TripBooking[], sagas: SagaState[] }>(observer => {
          Promise.all([
            this.getTrips().toPromise(),
            this.getSagaStates().toPromise()
          ]).then(([trips, sagas]) => {
            this.trips.set(trips?.items ?? []);
            this.sagaStates.set(sagas?.items ?? []);
            observer.next({ trips: trips?.items ?? [], sagas: sagas?.items ?? [] });
            observer.complete();
          }).catch(err => {
            observer.error(err);
          });
        });
      })
    );
  }
}
