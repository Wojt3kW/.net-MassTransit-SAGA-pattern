namespace Trip.Contracts.Events;

public record TripBookingFailed(
    Guid TripId,
    string Reason,
    DateTime FailedAt);
