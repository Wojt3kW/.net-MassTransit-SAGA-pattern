namespace FlightBooking.Contracts.Commands;

public record ReserveReturnFlight(
    Guid CorrelationId,
    Guid TripId,
    string Origin,
    string Destination,
    DateTime DepartureDate,
    string FlightNumber,
    string Carrier,
    int PassengerCount);
