namespace Payment.Contracts.Events;

public record PaymentRefunded(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentId,
    decimal Amount,
    DateTime RefundedAt);
