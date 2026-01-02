namespace Trip.Contracts.Events;

/// <summary>Published when a trip booking has been cancelled by user request or system.</summary>
public record TripBookingCancelled(
    Guid TripId,
    string Reason,
    DateTime CancelledAt);
