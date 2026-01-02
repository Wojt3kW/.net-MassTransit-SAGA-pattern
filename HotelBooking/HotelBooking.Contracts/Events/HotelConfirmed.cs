namespace HotelBooking.Contracts.Events;

/// <summary>Published when a hotel reservation has been confirmed after initial hold.</summary>
public record HotelConfirmed(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId,
    DateTime ConfirmedAt);
