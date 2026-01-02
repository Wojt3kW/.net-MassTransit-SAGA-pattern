namespace Payment.Contracts.Events;

/// <summary>Published when payment has been captured (funds charged after booking completion).</summary>
public record PaymentCaptured(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentId,
    decimal Amount,
    DateTime CapturedAt);
