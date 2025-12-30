namespace Trip.Contracts.Events;

public record TripBookingCancelled(
    Guid TripId,
    string Reason,
    DateTime CancelledAt);
