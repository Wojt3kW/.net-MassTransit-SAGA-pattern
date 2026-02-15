import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { interval, Subscription } from 'rxjs';
import { TripService } from '../../core/services/trip.service';
import {
  SAGA_STATE_COLORS,
  SagaState,
  FlightReservation,
  HotelReservation,
  PaymentTransaction,
} from '../../core/models/trip.models';

/// <summary>
/// Trip details page displaying SAGA timeline, reservation status, and related data.
/// </summary>
@Component({
  selector: 'app-trip-details',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressBarModule,
    MatDividerModule,
    MatListModule,
    MatTabsModule,
    MatTableModule,
    MatTooltipModule,
    MatSnackBarModule,
    MatDialogModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './trip-details.component.html',
  styleUrl: './trip-details.component.scss',
})
export class TripDetailsComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private tripService = inject(TripService);
  private snackBar = inject(MatSnackBar);
  private pollingSubscription?: Subscription;

  tripId = '';
  loading = signal(true);
  error = signal<string | null>(null);
  sagaState = signal<SagaState | null>(null);
  flights = signal<FlightReservation[]>([]);
  hotel = signal<HotelReservation | null>(null);
  payment = signal<PaymentTransaction | null>(null);

  /// <summary>
  /// Computes the SAGA flow visualization showing happy path and compensation steps.
  /// </summary>
  sagaFlow = computed(() => {
    const state = this.sagaState();
    if (!state) return { path: [], compensationPath: [], failedStep: null };

    const currentState = state.currentState || '';
    const isFailed = currentState === 'Failed';
    const isCancelled = currentState === 'Cancelled';
    const isCompensating =
      currentState.startsWith('Compensating') || currentState === 'ReleasingPayment';

    // Use ID presence to determine if step was originally completed (before compensation)
    // Boolean flags are reset during compensation, but IDs remain
    const wasOutboundReserved = !!state.outboundFlightId;
    const wasReturnReserved = !!state.returnFlightId;
    const wasHotelReserved = !!state.hotelReservationId;
    const wasTransportReserved = !!state.groundTransportId;
    const wasInsuranceIssued = !!state.insurancePolicyId;
    const wasPaymentAuthorised = !!state.paymentTransactionId;

    // Define all happy path steps with their completion status
    const allSteps = [
      {
        id: 'payment-auth',
        name: 'Authorise Payment',
        icon: 'credit_card',
        completed: wasPaymentAuthorised,
      },
      {
        id: 'outbound',
        name: 'Reserve Outbound Flight',
        icon: 'flight_takeoff',
        completed: wasOutboundReserved,
      },
      {
        id: 'return',
        name: 'Reserve Return Flight',
        icon: 'flight_land',
        completed: wasReturnReserved,
      },
      { id: 'hotel', name: 'Reserve Hotel', icon: 'hotel', completed: wasHotelReserved },
      {
        id: 'hotel-confirm',
        name: 'Confirm Hotel',
        icon: 'verified',
        completed: state.isHotelConfirmed,
      },
      {
        id: 'transport',
        name: 'Reserve Transport',
        icon: 'directions_car',
        completed: wasTransportReserved,
        optional: !state.includeGroundTransport,
      },
      {
        id: 'insurance',
        name: 'Issue Insurance',
        icon: 'security',
        completed: wasInsuranceIssued,
        optional: !state.includeInsurance,
      },
      {
        id: 'payment-capture',
        name: 'Capture Payment',
        icon: 'payment',
        completed: state.isPaymentCaptured,
      },
      {
        id: 'complete',
        name: 'Booking Complete',
        icon: 'check_circle',
        completed: currentState === 'Completed' || currentState === 'Refunded',
      },
    ];

    // Filter out optional steps that weren't included
    const path = allSteps.filter((step) => !step.optional || step.completed);

    // Find the failed step (first incomplete step when failed)
    let failedStep: string | null = null;
    if (isFailed || isCancelled || isCompensating) {
      const failedIndex = path.findIndex((step) => !step.completed && !step.optional);
      if (failedIndex >= 0) {
        failedStep = path[failedIndex].id;
      }
    }

    // Build compensation path based on what was completed (use ID presence)
    // Boolean flags are reset during compensation, so we check if resource ID exists
    const compensationSteps: Array<{ id: string; name: string; icon: string; completed: boolean }> =
      [];

    if (isFailed || isCancelled || isCompensating) {
      // Insurance compensation - only if insurance was issued
      if (wasInsuranceIssued) {
        compensationSteps.push({
          id: 'cancel-insurance',
          name: 'Cancel Insurance',
          icon: 'security',
          completed: !state.isInsuranceIssued || isFailed,
        });
      }
      // Ground transport compensation - only if transport was reserved
      if (wasTransportReserved) {
        compensationSteps.push({
          id: 'cancel-transport',
          name: 'Cancel Transport',
          icon: 'directions_car',
          completed: !state.isGroundTransportReserved || isFailed,
        });
      }
      // Hotel compensation - only if hotel was reserved
      if (wasHotelReserved) {
        compensationSteps.push({
          id: 'cancel-hotel',
          name: 'Cancel Hotel',
          icon: 'hotel',
          completed: !state.isHotelReserved || isFailed,
        });
      }
      // Return flight compensation - only if return flight was reserved
      if (wasReturnReserved) {
        compensationSteps.push({
          id: 'cancel-return',
          name: 'Cancel Return Flight',
          icon: 'flight_land',
          completed: !state.isReturnFlightReserved || isFailed,
        });
      }
      // Outbound flight compensation - only if outbound flight was reserved
      if (wasOutboundReserved) {
        compensationSteps.push({
          id: 'cancel-outbound',
          name: 'Cancel Outbound Flight',
          icon: 'flight_takeoff',
          completed: !state.isOutboundFlightReserved || isFailed,
        });
      }
      // Payment release - only if payment was authorised
      if (wasPaymentAuthorised) {
        compensationSteps.push({
          id: 'release-payment',
          name: 'Release Payment',
          icon: 'money_off',
          completed: !state.isPaymentAuthorised || isFailed,
        });
      }
    }

    return {
      path: path.map((step) => ({
        ...step,
        status: this.getFlowStepStatus(step, currentState, failedStep),
      })),
      compensationPath: compensationSteps.map((step, index) => ({
        ...step,
        status: this.getCompensationStepStatus(step, currentState, index, compensationSteps.length),
      })),
      failedStep,
      showCompensation: isFailed || isCancelled || isCompensating,
    };
  });

  /// <summary>
  /// Determines the status of a path step.
  /// </summary>
  private getFlowStepStatus(
    step: { id: string; completed: boolean; optional?: boolean },
    currentState: string,
    failedStep: string | null,
  ): 'completed' | 'active' | 'failed' | 'pending' | 'skipped' {
    if (step.completed) return 'completed';
    if (step.optional) return 'skipped';
    if (step.id === failedStep) return 'failed';

    const isActive =
      currentState.startsWith('Awaiting') &&
      ((step.id === 'payment-auth' && currentState === 'AwaitingPaymentAuthorisation') ||
        (step.id === 'outbound' && currentState === 'AwaitingOutboundFlight') ||
        (step.id === 'return' && currentState === 'AwaitingReturnFlight') ||
        (step.id === 'hotel' && currentState === 'AwaitingHotel') ||
        (step.id === 'hotel-confirm' && currentState === 'AwaitingHotelConfirmation') ||
        (step.id === 'transport' && currentState === 'AwaitingGroundTransport') ||
        (step.id === 'insurance' && currentState === 'AwaitingInsurance') ||
        (step.id === 'payment-capture' && currentState === 'AwaitingPaymentCapture'));

    if (isActive) return 'active';
    return 'pending';
  }

  /// <summary>
  /// Determines the status of a compensation step.
  /// </summary>
  private getCompensationStepStatus(
    step: { id: string; completed: boolean },
    currentState: string,
    index: number,
    totalSteps: number,
  ): 'completed' | 'active' | 'pending' {
    if (currentState === 'Failed' || currentState === 'Cancelled') return 'completed';

    // Determine which compensation step is active based on current state
    const compensationStateMap: Record<string, string> = {
      CompensatingInsurance: 'cancel-insurance',
      CompensatingGroundTransport: 'cancel-transport',
      CompensatingHotel: 'cancel-hotel',
      CompensatingReturnFlight: 'cancel-return',
      CompensatingOutboundFlight: 'cancel-outbound',
      ReleasingPayment: 'release-payment',
    };

    const activeCompensationStep = compensationStateMap[currentState];
    if (step.id === activeCompensationStep) return 'active';

    // Steps before active are completed
    const activeIndex = Object.values(compensationStateMap).indexOf(activeCompensationStep);
    const stepIndex = Object.values(compensationStateMap).indexOf(step.id);
    if (activeIndex >= 0 && stepIndex < activeIndex) return 'completed';

    return 'pending';
  }

  sagaSteps = computed(() => {
    const state = this.sagaState();
    if (!state) return [];

    const currentState = state.currentState || '';

    // Use ID presence to determine if step was originally completed (before compensation)
    // Boolean flags are reset during compensation, but IDs remain
    const steps = [
      {
        name: 'Payment Auth',
        icon: 'credit_card',
        statePrefix: 'AwaitingPaymentAuthorisation',
        isCompleted: !!state.paymentTransactionId,
      },
      {
        name: 'Outbound Flight',
        icon: 'flight_takeoff',
        statePrefix: 'AwaitingOutboundFlight',
        isCompleted: !!state.outboundFlightId,
      },
      {
        name: 'Return Flight',
        icon: 'flight_land',
        statePrefix: 'AwaitingReturnFlight',
        isCompleted: !!state.returnFlightId,
      },
      {
        name: 'Hotel',
        icon: 'hotel',
        statePrefix: 'AwaitingHotel',
        isCompleted: !!state.hotelReservationId,
      },
      {
        name: 'Transport',
        icon: 'directions_car',
        statePrefix: 'AwaitingGroundTransport',
        isCompleted: !!state.groundTransportId,
        isOptional: !state.includeGroundTransport,
      },
      {
        name: 'Insurance',
        icon: 'security',
        statePrefix: 'AwaitingInsurance',
        isCompleted: !!state.insurancePolicyId,
        isOptional: !state.includeInsurance,
      },
      {
        name: 'Payment Capture',
        icon: 'payment',
        statePrefix: 'AwaitingPaymentCapture',
        isCompleted: state.isPaymentCaptured,
      },
      {
        name: 'Complete',
        icon: 'check_circle',
        statePrefix: 'Completed',
        isCompleted: currentState === 'Completed' || currentState === 'Refunded',
      },
    ];

    const isFailed = currentState === 'Failed' || currentState.startsWith('Compensating');
    const isCancelled = currentState === 'Cancelled';

    // Find which step actually failed (first incomplete, non-optional step)
    let failedStepIndex = -1;
    if (isFailed || isCancelled) {
      failedStepIndex = steps.findIndex((s) => !s.isCompleted && !s.isOptional);
    }

    return steps.map((step, index) => ({
      ...step,
      completed: step.isCompleted,
      active:
        currentState.startsWith(step.statePrefix) ||
        (currentState.startsWith('AwaitingHotelConfirmation') &&
          step.statePrefix === 'AwaitingHotel'),
      failed: (isFailed || isCancelled) && index >= failedStepIndex && failedStepIndex >= 0,
      status: this.getStepStatusFromFlags(step, currentState),
    }));
  });

  ngOnInit(): void {
    this.tripId = this.route.snapshot.paramMap.get('id') || '';
    if (this.tripId) {
      this.loadTripDetails();
      this.startPolling();
    } else {
      this.error.set('No trip ID provided');
      this.loading.set(false);
    }
  }

  ngOnDestroy(): void {
    this.pollingSubscription?.unsubscribe();
  }

  private loadTripDetails(): void {
    this.tripService.getSagaState(this.tripId).subscribe({
      next: (state) => {
        this.sagaState.set(state);
        this.loading.set(false);
        this.loadRelatedData(state);
      },
      error: (err) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }

  private loadRelatedData(state: SagaState): void {
    this.tripService.getFlightsByTripId(this.tripId).subscribe({
      next: (response: any) => this.flights.set(response.items || []),
      error: () => {},
    });

    this.tripService.getHotelsByTripId(this.tripId).subscribe({
      next: (response: any) => this.hotel.set(response.items?.[0] || null),
      error: () => {},
    });

    this.tripService.getPaymentsByTripId(this.tripId).subscribe({
      next: (response: any) => this.payment.set(response.items?.[0] || null),
      error: () => {},
    });
  }

  private startPolling(): void {
    this.pollingSubscription = interval(2000).subscribe(() => {
      this.tripService.getSagaState(this.tripId).subscribe({
        next: (state) => {
          this.sagaState.set(state);
          this.loadRelatedData(state);
        },
      });
    });
  }

  /// <summary>
  /// Determines step status based on SAGA state flags and current state.
  /// </summary>
  private getStepStatusFromFlags(
    step: { statePrefix: string; isCompleted: boolean; isOptional?: boolean },
    currentState: string,
  ): string {
    const isFailed = currentState === 'Failed';
    const isCancelled = currentState === 'Cancelled';
    const isCompensating = currentState.startsWith('Compensating');
    const isActive =
      currentState.startsWith(step.statePrefix) ||
      (currentState.startsWith('AwaitingHotelConfirmation') &&
        step.statePrefix === 'AwaitingHotel');

    // Final state - Completed
    if (currentState === 'Completed' && step.statePrefix === 'Completed') return 'Done';
    if (currentState === 'Completed') return 'Completed';

    // Step is currently active (in progress)
    if (isActive) return 'In Progress';

    // Step was completed (based on boolean flags)
    if (step.isCompleted) {
      if (isFailed || isCancelled) return 'Compensated';
      if (isCompensating) return 'Compensating';
      return 'Completed';
    }

    // Step was skipped (optional and not completed)
    if (step.isOptional) return 'Skipped';

    // Failed/Cancelled without completion = Failed at this step or earlier
    if (isFailed) return 'Failed';
    if (isCancelled) return 'Cancelled';
    if (isCompensating) return 'Pending';

    return 'Pending';
  }

  getStateColor(): string {
    const state = this.sagaState()?.currentState || '';
    return SAGA_STATE_COLORS[state] || '#9e9e9e';
  }

  isInProgress(): boolean {
    const state = this.sagaState()?.currentState || '';
    return state.startsWith('Awaiting') || state.startsWith('Compensating');
  }

  getProgress(): number {
    const state = this.sagaState()?.currentState || '';
    if (state === 'Completed') return 100;
    if (state === 'Failed' || state === 'Cancelled') return 100;

    const stateOrder = [
      'AwaitingPaymentAuthorisation',
      'AwaitingOutboundFlight',
      'AwaitingReturnFlight',
      'AwaitingHotel',
      'AwaitingHotelConfirmation',
      'AwaitingGroundTransport',
      'AwaitingInsurance',
      'AwaitingPaymentCapture',
    ];

    const index = stateOrder.findIndex((s) => state.startsWith(s));
    return index >= 0 ? ((index + 1) / stateOrder.length) * 100 : 0;
  }

  getProgressColor(): 'primary' | 'accent' | 'warn' {
    const state = this.sagaState()?.currentState || '';
    if (state === 'Failed' || state === 'Cancelled') return 'warn';
    if (state === 'Completed') return 'accent';
    return 'primary';
  }

  canCancel(): boolean {
    const state = this.sagaState()?.currentState || '';
    return (
      state.startsWith('Awaiting') &&
      state !== 'AwaitingPaymentAuthorisation' &&
      !state.startsWith('Compensating')
    );
  }

  cancelTrip(): void {
    if (!confirm('Are you sure you want to cancel this trip?')) return;

    this.tripService.cancelTrip(this.tripId, 'User requested cancellation').subscribe({
      next: () => {
        this.snackBar.open('Cancellation request sent', 'OK', { duration: 3000 });
      },
      error: (err) => {
        this.snackBar.open('Failed to cancel: ' + err.message, 'Close', { duration: 5000 });
      },
    });
  }

  /// <summary>
  /// Requests a refund for a completed trip.
  /// </summary>
  refundTrip(): void {
    if (!confirm('Are you sure you want to request a refund for this trip?')) return;

    this.tripService.refundTrip(this.tripId, 'User requested refund').subscribe({
      next: () => {
        this.snackBar.open('Refund request sent', 'OK', { duration: 3000 });
      },
      error: (err) => {
        this.snackBar.open('Failed to refund: ' + err.message, 'Close', { duration: 5000 });
      },
    });
  }

  /// <summary>
  /// Checks if the current trip can be refunded (only completed trips).
  /// </summary>
  canRefund(): boolean {
    const state = this.sagaState();
    return state?.currentState === 'Completed';
  }
}
