namespace Trip.Contracts.Commands;

public record CancelTripBooking(
    Guid TripId,
    string Reason);
