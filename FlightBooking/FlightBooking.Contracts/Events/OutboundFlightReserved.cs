namespace FlightBooking.Contracts.Events;

public record OutboundFlightReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    string FlightNumber,
    string ConfirmationCode,
    DateTime DepartureDate,
    decimal Price);
