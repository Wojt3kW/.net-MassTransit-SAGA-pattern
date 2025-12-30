namespace FlightBooking.Contracts.Events;

public record FlightCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    DateTime CancelledAt);
