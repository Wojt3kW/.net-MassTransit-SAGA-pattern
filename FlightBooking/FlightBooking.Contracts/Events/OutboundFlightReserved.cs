namespace FlightBooking.Contracts.Events;

/// <summary>Published when the outbound (departure) flight has been successfully reserved.</summary>
public record OutboundFlightReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    string FlightNumber,
    string ConfirmationCode,
    DateTime DepartureDate,
    decimal Price);
