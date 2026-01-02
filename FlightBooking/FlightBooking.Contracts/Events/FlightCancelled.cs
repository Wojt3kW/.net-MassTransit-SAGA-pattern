namespace FlightBooking.Contracts.Events;

/// <summary>Published when a flight reservation has been successfully cancelled during compensation.</summary>
public record FlightCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    DateTime CancelledAt);
