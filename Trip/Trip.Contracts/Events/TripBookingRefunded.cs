namespace Trip.Contracts.Events;

/// <summary>Published when a trip booking has been fully refunded.</summary>
public record TripBookingRefunded(
    Guid TripId,
    decimal RefundedAmount,
    DateTime RefundedAt);
