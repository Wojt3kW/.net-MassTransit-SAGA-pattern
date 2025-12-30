namespace Trip.Contracts.Events;

public record TripBookingCompleted(
    Guid TripId,
    DateTime CompletedAt);
