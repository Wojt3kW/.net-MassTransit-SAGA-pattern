namespace FlightBooking.Contracts.Events;

public record ReturnFlightReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    string FlightNumber,
    string ConfirmationCode,
    DateTime DepartureDate,
    decimal Price);
