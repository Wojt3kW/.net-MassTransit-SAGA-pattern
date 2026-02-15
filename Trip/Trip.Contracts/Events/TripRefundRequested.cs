namespace Trip.Contracts.Events;

/// <summary>Published when a customer requests a refund for a completed trip.</summary>
public record TripRefundRequested(
    Guid TripId,
    string Reason,
    DateTime RequestedAt);
