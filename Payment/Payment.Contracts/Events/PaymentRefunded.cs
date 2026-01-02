namespace Payment.Contracts.Events;

/// <summary>Published when a captured payment has been refunded to the customer.</summary>
public record PaymentRefunded(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentId,
    decimal Amount,
    DateTime RefundedAt);
