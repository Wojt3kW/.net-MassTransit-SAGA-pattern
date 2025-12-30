namespace Payment.Contracts.Events;

public record PaymentCaptured(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentId,
    decimal Amount,
    DateTime CapturedAt);
