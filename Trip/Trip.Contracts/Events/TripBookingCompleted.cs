namespace Trip.Contracts.Events;

/// <summary>Published when a trip booking has been successfully completed with all services confirmed.</summary>
public record TripBookingCompleted(
    Guid TripId,
    DateTime CompletedAt);
