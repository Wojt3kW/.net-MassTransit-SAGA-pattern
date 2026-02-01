import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { Subscription } from 'rxjs';
import { TripService } from '../../core/services/trip.service';
import { SagaState, SAGA_STATE_COLORS } from '../../core/models/trip.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatBadgeModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit, OnDestroy {
  tripService = inject(TripService);

  displayedColumns = ['state', 'customer', 'route', 'dates', 'progress', 'amount', 'actions'];
  useSSE = true;
  private sseSubscription?: Subscription;
  private pollingSubscription?: Subscription;

  ngOnInit(): void {
    // Load initial data
    this.tripService.loadTrips();
    this.tripService.loadSagaStates();

    // Use SSE for real-time saga updates
    if (this.useSSE) {
      this.startSSE();
    } else {
      this.startPolling();
    }
  }

  ngOnDestroy(): void {
    this.stopSSE();
    this.stopPolling();
  }

  startSSE(): void {
    this.useSSE = true;
    this.sseSubscription = this.tripService.startSagaSSE().subscribe({
      error: (err) => {
        console.error('SSE error, falling back to polling:', err);
        this.startPolling();
      }
    });
  }

  stopSSE(): void {
    this.sseSubscription?.unsubscribe();
    this.tripService.stopSagaSSE();
  }

  startPolling(): void {
    this.useSSE = false;
    this.pollingSubscription = this.tripService.startPolling(2000).subscribe();
  }

  stopPolling(): void {
    this.pollingSubscription?.unsubscribe();
  }

  toggleMode(): void {
    if (this.useSSE) {
      this.stopSSE();
      this.startPolling();
    } else {
      this.stopPolling();
      this.startSSE();
    }
  }

  refresh(): void {
    this.tripService.loadTrips();
    this.tripService.loadSagaStates();
  }

  getStateColor(state: string): string {
    return SAGA_STATE_COLORS[state] || '#9E9E9E';
  }

  isCompensatingState(state: string): boolean {
    return state.startsWith('Compensating') || state === 'ReleasingPayment';
  }

  isTerminalState(state: string): boolean {
    return ['Completed', 'Failed', 'Cancelled', 'TimedOut'].includes(state);
  }

  cancelTrip(tripId: string): void {
    if (confirm('Are you sure you want to cancel this trip?')) {
      this.tripService.cancelTrip(tripId, 'Cancelled by user via dashboard').subscribe({
        next: () => {
          this.refresh();
        },
        error: (err) => {
          alert('Failed to cancel trip: ' + err.message);
        }
      });
    }
  }
}
