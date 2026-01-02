namespace TripBooking.Saga.API.Features.GetSagaState;

/// <summary>
/// Response containing the current state of a saga.
/// </summary>
public record SagaStateResponse(
    Guid CorrelationId,
    Guid TripId,
    string CurrentState,
    Guid CustomerId,
    string CustomerEmail,
    string Origin,
    string Destination,
    DateTime DepartureDate,
    DateTime ReturnDate,
    decimal TotalAmount,
    Guid? PaymentTransactionId,
    Guid? OutboundFlightId,
    Guid? ReturnFlightId,
    Guid? HotelReservationId,
    Guid? GroundTransportId,
    Guid? InsurancePolicyId,
    bool IsPaymentAuthorised,
    bool IsOutboundFlightReserved,
    bool IsReturnFlightReserved,
    bool IsHotelReserved,
    bool IsHotelConfirmed,
    bool IsGroundTransportReserved,
    bool IsInsuranceIssued,
    bool IsPaymentCaptured,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? FailureReason
);
