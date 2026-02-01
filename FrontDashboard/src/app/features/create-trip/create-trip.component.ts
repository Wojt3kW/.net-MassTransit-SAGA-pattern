import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatStepperModule } from '@angular/material/stepper';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { TripService } from '../../core/services/trip.service';
import { CreateTripRequest } from '../../core/models/trip.models';

@Component({
  selector: 'app-create-trip',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatStepperModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDividerModule
  ],
  templateUrl: './create-trip.component.html',
  styleUrl: './create-trip.component.scss'
})
export class CreateTripComponent {
  private fb = inject(FormBuilder);
  private tripService = inject(TripService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  isSubmitting = false;

  customerForm: FormGroup = this.fb.group({
    customerName: ['John Smith', Validators.required],
    customerEmail: ['john.smith@email.com', [Validators.required, Validators.email]]
  });

  flightsForm: FormGroup = this.fb.group({
    outboundOrigin: ['LHR', Validators.required],
    outboundDestination: ['JFK', Validators.required],
    outboundFlightNumber: ['BA178', Validators.required],
    outboundCarrier: ['British Airways', Validators.required],
    outboundDate: [new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), Validators.required],
    returnOrigin: ['JFK', Validators.required],
    returnDestination: ['LHR', Validators.required],
    returnFlightNumber: ['BA179', Validators.required],
    returnCarrier: ['British Airways', Validators.required],
    returnDate: [new Date(Date.now() + 37 * 24 * 60 * 60 * 1000), Validators.required]
  });

  hotelForm: FormGroup = this.fb.group({
    hotelName: ['The Plaza Hotel', Validators.required],
    checkIn: [new Date(Date.now() + 30 * 24 * 60 * 60 * 1000), Validators.required],
    checkOut: [new Date(Date.now() + 37 * 24 * 60 * 60 * 1000), Validators.required],
    numberOfGuests: [2, [Validators.required, Validators.min(1)]]
  });

  paymentForm: FormGroup = this.fb.group({
    includeGroundTransport: [false],
    includeInsurance: [true],
    transportType: ['Airport Transfer'],
    cardNumber: ['4242424242424242', Validators.required],
    expiryDate: ['12/28', Validators.required],
    cvv: ['123', Validators.required],
    amount: [2500, [Validators.required, Validators.min(0.01)]]
  });

  submitBooking(): void {
    if (this.customerForm.invalid || this.flightsForm.invalid ||
        this.hotelForm.invalid || this.paymentForm.invalid) {
      this.snackBar.open('Please fill in all required fields', 'Close', { duration: 3000 });
      return;
    }

    this.isSubmitting = true;

    const request: CreateTripRequest = {
      customerId: 'c1111111-1111-1111-1111-111111111111',
      customerEmail: this.customerForm.get('customerEmail')?.value,
      customerName: this.customerForm.get('customerName')?.value,
      details: {
        outboundFlight: {
          origin: this.flightsForm.get('outboundOrigin')?.value,
          destination: this.flightsForm.get('outboundDestination')?.value,
          departureDate: this.formatDate(this.flightsForm.get('outboundDate')?.value),
          flightNumber: this.flightsForm.get('outboundFlightNumber')?.value,
          carrier: this.flightsForm.get('outboundCarrier')?.value
        },
        returnFlight: {
          origin: this.flightsForm.get('returnOrigin')?.value,
          destination: this.flightsForm.get('returnDestination')?.value,
          departureDate: this.formatDate(this.flightsForm.get('returnDate')?.value),
          flightNumber: this.flightsForm.get('returnFlightNumber')?.value,
          carrier: this.flightsForm.get('returnCarrier')?.value
        },
        hotel: {
          hotelId: '245bccd2-f5f3-40e3-8dcf-885fec39aaaa',
          hotelName: this.hotelForm.get('hotelName')?.value,
          checkIn: this.formatDate(this.hotelForm.get('checkIn')?.value),
          checkOut: this.formatDate(this.hotelForm.get('checkOut')?.value),
          numberOfGuests: this.hotelForm.get('numberOfGuests')?.value
        },
        groundTransport: this.paymentForm.get('includeGroundTransport')?.value ? {
          type: this.paymentForm.get('transportType')?.value,
          pickupLocation: 'Airport Terminal',
          dropoffLocation: 'Hotel',
          pickupDateTime: this.formatDate(this.flightsForm.get('outboundDate')?.value),
          passengerCount: this.hotelForm.get('numberOfGuests')?.value
        } : undefined,
        includeInsurance: this.paymentForm.get('includeInsurance')?.value,
        paymentMethodId: 'a1111111-1111-1111-1111-111111111111',
        payment: {
          cardNumber: this.paymentForm.get('cardNumber')?.value,
          cardHolderName: this.customerForm.get('customerName')?.value,
          expiryDate: this.paymentForm.get('expiryDate')?.value,
          cvv: this.paymentForm.get('cvv')?.value,
          amount: this.paymentForm.get('amount')?.value,
          currency: 'GBP'
        }
      }
    };

    this.tripService.createTrip(request).subscribe({
      next: (trip) => {
        this.isSubmitting = false;
        this.snackBar.open('Trip booking started! SAGA is now running.', 'View', { duration: 5000 })
          .onAction().subscribe(() => {
            this.router.navigate(['/trip', trip.id]);
          });
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.snackBar.open('Failed to create trip: ' + err.message, 'Close', { duration: 5000 });
      }
    });
  }

  private formatDate(date: Date): string {
    return date.toISOString();
  }
}
