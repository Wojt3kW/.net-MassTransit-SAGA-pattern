namespace Trip.Contracts.Events;

/// <summary>Published when a trip booking has failed and compensation has been completed.</summary>
public record TripBookingFailed(
    Guid TripId,
    string Reason,
    DateTime FailedAt);
