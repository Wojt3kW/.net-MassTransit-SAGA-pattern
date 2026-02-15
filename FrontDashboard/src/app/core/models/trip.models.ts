// Trip Booking Models

export interface TripBooking {
  id: string;
  customerId: string;
  customerEmail: string;
  customerName: string;
  status: TripStatus;
  outboundFlightStatus?: string;
  returnFlightStatus?: string;
  hotelStatus?: string;
  groundTransportStatus?: string;
  insuranceStatus?: string;
  paymentStatus?: string;
  totalAmount: number;
  createdAt: string;
  completedAt?: string;
  failureReason?: string;
}

export type TripStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed' | 'Cancelled' | 'Refunded';

export interface SagaState {
  correlationId: string;
  currentState: string;
  tripId: string;
  customerId: string;
  customerName: string;
  customerEmail: string;
  origin: string;
  destination: string;
  departureDate: string;
  returnDate: string;
  outboundFlightNumber: string;
  outboundCarrier: string;
  returnFlightNumber: string;
  returnCarrier: string;
  hotelId: string;
  hotelName: string;
  checkIn: string;
  checkOut: string;
  numberOfGuests: number;
  includeGroundTransport: boolean;
  includeInsurance: boolean;
  paymentMethodId: string;
  totalAmount: number;
  amount: number;
  currency: string;
  paymentTransactionId?: string;
  outboundFlightId?: string;
  outboundFlightReservationId?: string;
  returnFlightId?: string;
  returnFlightReservationId?: string;
  hotelReservationId?: string;
  groundTransportId?: string;
  transportReservationId?: string;
  insurancePolicyId?: string;
  isPaymentAuthorised: boolean;
  isOutboundFlightReserved: boolean;
  isReturnFlightReserved: boolean;
  isHotelReserved: boolean;
  isHotelConfirmed: boolean;
  isGroundTransportReserved: boolean;
  isInsuranceIssued: boolean;
  isPaymentCaptured: boolean;
  isCancelledByUser: boolean;
  paymentCaptureRetryCount: number;
  failureReason?: string;
  createdAt: string;
}

export interface CreateTripRequest {
  customerId: string;
  customerEmail: string;
  customerName: string;
  details: TripDetails;
}

export interface TripDetails {
  outboundFlight: FlightDetails;
  returnFlight: FlightDetails;
  hotel: HotelDetails;
  groundTransport?: GroundTransportDetails;
  includeInsurance: boolean;
  paymentMethodId: string;
  payment: PaymentDetails;
}

export interface FlightDetails {
  origin: string;
  destination: string;
  departureDate: string;
  flightNumber: string;
  carrier: string;
}

export interface HotelDetails {
  hotelId: string;
  hotelName: string;
  checkIn: string;
  checkOut: string;
  numberOfGuests: number;
}

export interface GroundTransportDetails {
  type: string;
  pickupLocation: string;
  dropoffLocation: string;
  pickupDateTime: string;
  passengerCount: number;
}

export interface PaymentDetails {
  cardNumber: string;
  cardHolderName: string;
  expiryDate: string;
  cvv: string;
  amount: number;
  currency: string;
}

export interface CancelTripRequest {
  reason: string;
}

export interface RefundTripRequest {
  reason: string;
}

export interface GroundTransportReservation {
  id: string;
  tripId: string;
  type: string;
  pickupLocation: string;
  dropoffLocation: string;
  pickupDateTime: string;
  passengerCount: number;
  status: string;
  createdAt: string;
}

export interface InsurancePolicy {
  id: string;
  tripId: string;
  policyNumber: string;
  coverageAmount: number;
  premium: number;
  status: string;
  createdAt: string;
}

export interface FlightReservation {
  id: string;
  tripId: string;
  type: 'Outbound' | 'Return';
  isOutbound: boolean;
  flightNumber: string;
  carrier: string;
  origin: string;
  destination: string;
  departureDate: string;
  confirmationCode: string;
  price: number;
  passengerCount: number;
  status: string;
  cancellationReason?: string;
  createdAt: string;
  confirmedAt?: string;
  cancelledAt?: string;
}

export interface HotelReservation {
  id: string;
  tripId: string;
  hotelId: string;
  hotelName: string;
  checkIn: string;
  checkOut: string;
  numberOfGuests: number;
  guestName: string;
  guestEmail: string;
  confirmationCode: string;
  pricePerNight: number;
  totalPrice: number;
  status: string;
  cancellationReason?: string;
  createdAt: string;
  confirmedAt?: string;
  cancelledAt?: string;
  expiresAt?: string;
}

export interface PaymentTransaction {
  id: string;
  tripId: string;
  customerId: string;
  cardLast4: string;
  cardLastFourDigits: string;
  cardHolderName: string;
  amount: number;
  currency: string;
  authorisationCode?: string;
  status: string;
  failureReason?: string;
  createdAt: string;
  authorisedAt?: string;
  capturedAt?: string;
  releasedAt?: string;
  refundedAt?: string;
}

export interface PagedSagaResponse {
  items: SagaState[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface PagedTripsResponse {
  items: TripBooking[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

// State colors for UI
export const SAGA_STATE_COLORS: Record<string, string> = {
  'AwaitingPaymentAuthorisation': '#FFA726',
  'AwaitingOutboundFlight': '#42A5F5',
  'AwaitingReturnFlight': '#42A5F5',
  'AwaitingHotel': '#7E57C2',
  'AwaitingHotelConfirmation': '#7E57C2',
  'AwaitingGroundTransport': '#26A69A',
  'AwaitingInsurance': '#5C6BC0',
  'AwaitingPaymentCapture': '#FFA726',
  'AwaitingPaymentRefund': '#9C27B0',
  'Completed': '#66BB6A',
  'Refunded': '#9C27B0',
  'Failed': '#EF5350',
  'Cancelled': '#BDBDBD',
  'TimedOut': '#FF7043',
  'ManualReviewRequired': '#FFB300',
  'CompensatingInsurance': '#EF5350',
  'CompensatingGroundTransport': '#EF5350',
  'CompensatingHotel': '#EF5350',
  'CompensatingReturnFlight': '#EF5350',
  'CompensatingOutboundFlight': '#EF5350',
  'ReleasingPayment': '#EF5350',
};
