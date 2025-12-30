namespace Trip.API.Features.CreateTrip;

public record TripDetailsDto(
    FlightDetailsDto OutboundFlight,
    FlightDetailsDto ReturnFlight,
    HotelDetailsDto Hotel,
    GroundTransportDetailsDto? GroundTransport,
    bool IncludeInsurance,
    PaymentDetailsDto Payment);
