namespace FlightBooking.Contracts.Events;

/// <summary>Published when a flight reservation request has failed (e.g., no availability, invalid data).</summary>
public record FlightReservationFailed(
    Guid CorrelationId,
    Guid TripId,
    string FlightNumber,
    string Reason);
