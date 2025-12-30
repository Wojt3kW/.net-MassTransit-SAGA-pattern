namespace Trip.Contracts.Events;

public record TripBookingCreated(
    Guid TripId,
    Guid CustomerId,
    DateTime CreatedAt);
