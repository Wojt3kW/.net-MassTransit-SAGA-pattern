namespace Trip.Contracts.Commands;

public record TripDetails(
    FlightDetails OutboundFlight,
    FlightDetails ReturnFlight,
    HotelDetails Hotel,
    GroundTransportDetails? GroundTransport,
    bool IncludeInsurance,
    PaymentDetails Payment);
