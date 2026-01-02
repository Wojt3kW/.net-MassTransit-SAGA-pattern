namespace FlightBooking.Contracts.Events;

/// <summary>Published when the return flight has been successfully reserved.</summary>
public record ReturnFlightReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid FlightReservationId,
    string FlightNumber,
    string ConfirmationCode,
    DateTime DepartureDate,
    decimal Price);
