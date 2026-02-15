using MassTransit;

namespace TripBooking.Saga.States
{
    /// <summary>
    /// Represents the state of a trip booking orchestration saga.
    /// </summary>
    public class TripBookingSagaState : SagaStateMachineInstance
    {
        /// <summary>Unique identifier for the saga instance.</summary>
        public Guid CorrelationId { get; set; }

        /// <summary>Current state of the saga state machine.</summary>
        public string CurrentState { get; set; } = default!;

        /// <summary>Identifier of the trip booking in Trip.API.</summary>
        public Guid TripId { get; set; }

        /// <summary>Identifier of the customer making the booking.</summary>
        public Guid CustomerId { get; set; }

        /// <summary>Full name of the customer.</summary>
        public string CustomerName { get; set; } = default!;

        /// <summary>Email address for customer notifications.</summary>
        public string CustomerEmail { get; set; } = default!;

        /// <summary>Departure city or airport code.</summary>
        public string Origin { get; set; } = default!;

        /// <summary>Arrival city or airport code.</summary>
        public string Destination { get; set; } = default!;

        /// <summary>Date of outbound flight departure.</summary>
        public DateTime DepartureDate { get; set; }

        /// <summary>Date of return flight departure.</summary>
        public DateTime ReturnDate { get; set; }

        /// <summary>Outbound flight number.</summary>
        public string OutboundFlightNumber { get; set; } = default!;

        /// <summary>Outbound flight carrier.</summary>
        public string OutboundCarrier { get; set; } = default!;

        /// <summary>Return flight number.</summary>
        public string ReturnFlightNumber { get; set; } = default!;

        /// <summary>Return flight carrier.</summary>
        public string ReturnCarrier { get; set; } = default!;

        /// <summary>Hotel identifier.</summary>
        public Guid HotelId { get; set; } = default!;

        /// <summary>Hotel name.</summary>
        public string HotelName { get; set; } = default!;

        /// <summary>Hotel check-in date.</summary>
        public DateTime CheckIn { get; set; }

        /// <summary>Hotel check-out date.</summary>
        public DateTime CheckOut { get; set; }

        /// <summary>Number of hotel guests.</summary>
        public int NumberOfGuests { get; set; }

        /// <summary>Whether ground transport is included.</summary>
        public bool IncludeGroundTransport { get; set; }

        /// <summary>Type of ground transport (if included).</summary>
        public string? GroundTransportType { get; set; }

        /// <summary>Ground transport pickup location.</summary>
        public string? GroundTransportPickupLocation { get; set; }

        /// <summary>Ground transport dropoff location.</summary>
        public string? GroundTransportDropoffLocation { get; set; }

        /// <summary>Whether travel insurance is included.</summary>
        public bool IncludeInsurance { get; set; }

        /// <summary>Customer's payment method identifier.</summary>
        public Guid? PaymentMethodId { get; set; }

        /// <summary>Total booking amount in the base currency.</summary>
        public decimal TotalAmount { get; set; }

        /// <summary>Currency code (e.g., "GBP", "USD").</summary>
        public string Currency { get; set; } = default!;

        /// <summary>Payment transaction identifier from Payment.API.</summary>
        public Guid? PaymentTransactionId { get; set; }

        /// <summary>Outbound flight reservation identifier from FlightBooking.API.</summary>
        public Guid? OutboundFlightId { get; set; }

        /// <summary>Return flight reservation identifier from FlightBooking.API.</summary>
        public Guid? ReturnFlightId { get; set; }

        /// <summary>Hotel reservation identifier from HotelBooking.API.</summary>
        public Guid? HotelReservationId { get; set; }

        /// <summary>Ground transport reservation identifier from GroundTransport.API.</summary>
        public Guid? GroundTransportId { get; set; }

        /// <summary>Insurance policy identifier from Insurance.API.</summary>
        public Guid? InsurancePolicyId { get; set; }

        /// <summary>Indicates whether payment has been successfully authorised.</summary>
        public bool IsPaymentAuthorised { get; set; }

        /// <summary>Indicates whether outbound flight has been successfully reserved.</summary>
        public bool IsOutboundFlightReserved { get; set; }

        /// <summary>Indicates whether return flight has been successfully reserved.</summary>
        public bool IsReturnFlightReserved { get; set; }

        /// <summary>Indicates whether hotel has been successfully reserved.</summary>
        public bool IsHotelReserved { get; set; }

        /// <summary>Indicates whether hotel reservation has been confirmed.</summary>
        public bool IsHotelConfirmed { get; set; }

        /// <summary>Indicates whether ground transport has been successfully reserved.</summary>
        public bool IsGroundTransportReserved { get; set; }

        /// <summary>Indicates whether travel insurance has been successfully issued.</summary>
        public bool IsInsuranceIssued { get; set; }

        /// <summary>Indicates whether payment has been successfully captured.</summary>
        public bool IsPaymentCaptured { get; set; }

        /// <summary>Token for scheduled payment authorisation timeout.</summary>
        public Guid? PaymentAuthorisationTimeoutToken { get; set; }

        /// <summary>Token for scheduled outbound flight timeout.</summary>
        public Guid? OutboundFlightTimeoutToken { get; set; }

        /// <summary>Token for scheduled return flight timeout.</summary>
        public Guid? ReturnFlightTimeoutToken { get; set; }

        /// <summary>Token for scheduled hotel reservation timeout.</summary>
        public Guid? HotelReservationTimeoutToken { get; set; }

        /// <summary>Token for scheduled hotel confirmation timeout.</summary>
        public Guid? HotelConfirmationTimeoutToken { get; set; }

        /// <summary>Token for scheduled ground transport timeout.</summary>
        public Guid? GroundTransportTimeoutToken { get; set; }

        /// <summary>Token for scheduled insurance timeout.</summary>
        public Guid? InsuranceTimeoutToken { get; set; }

        /// <summary>Token for scheduled payment capture timeout.</summary>
        public Guid? PaymentCaptureTimeoutToken { get; set; }

        /// <summary>Token for scheduled user inactivity timeout.</summary>
        public Guid? UserInactivityTimeoutToken { get; set; }

        /// <summary>Number of payment capture retry attempts.</summary>
        public int PaymentCaptureRetryCount { get; set; }

        /// <summary>Reason for saga failure if applicable.</summary>
        public string? FailureReason { get; set; }

        /// <summary>Timestamp when the saga was created.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>Timestamp when the saga completed or failed.</summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>Indicates whether the saga was cancelled by the user.</summary>
        public bool IsCancelledByUser { get; set; }

        /// <summary>Indicates whether payment has been refunded.</summary>
        public bool IsRefunded { get; set; }

        /// <summary>Reason for refund request if applicable.</summary>
        public string? RefundReason { get; set; }
    }
}
