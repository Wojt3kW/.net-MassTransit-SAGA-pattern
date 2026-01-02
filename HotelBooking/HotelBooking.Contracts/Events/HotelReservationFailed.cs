namespace HotelBooking.Contracts.Events;

/// <summary>Published when a hotel reservation request has failed (e.g., no rooms available).</summary>
public record HotelReservationFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
