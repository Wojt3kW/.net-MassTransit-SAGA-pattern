namespace Payment.Contracts.Events;

/// <summary>Published when payment authorisation has been released (funds unblocked on compensation).</summary>
public record PaymentReleased(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    DateTime ReleasedAt,
    string Reason);
