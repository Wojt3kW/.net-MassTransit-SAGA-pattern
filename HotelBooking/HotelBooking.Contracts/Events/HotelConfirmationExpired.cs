namespace HotelBooking.Contracts.Events;

/// <summary>Published when a hotel reservation confirmation has timed out (15 min window expired).</summary>
public record HotelConfirmationExpired(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId);
