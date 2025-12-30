namespace FlightBooking.Contracts.Events;

public record FlightReservationFailed(
    Guid CorrelationId,
    Guid TripId,
    string FlightNumber,
    string Reason);
