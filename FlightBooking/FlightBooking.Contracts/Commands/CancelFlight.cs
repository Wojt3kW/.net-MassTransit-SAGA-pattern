namespace FlightBooking.Contracts.Commands;

public record CancelFlight(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    string Reason);
