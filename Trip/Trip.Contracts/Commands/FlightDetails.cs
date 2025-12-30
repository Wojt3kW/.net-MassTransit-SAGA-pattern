namespace Trip.Contracts.Commands;

public record FlightDetails(
    string Origin,
    string Destination,
    DateTime DepartureDate,
    string FlightNumber,
    string Carrier);
