namespace Trip.API.Features.CreateTrip;

public record FlightDetailsDto(
    string Origin,
    string Destination,
    DateTime DepartureDate,
    string FlightNumber,
    string Carrier);
