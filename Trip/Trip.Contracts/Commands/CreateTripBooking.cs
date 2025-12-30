namespace Trip.Contracts.Commands;

public record CreateTripBooking(
    Guid TripId,
    Guid CustomerId,
    string CustomerEmail,
    TripDetails Details);
