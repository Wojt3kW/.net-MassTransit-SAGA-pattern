namespace Payment.Contracts.Events;

public record PaymentCaptureFailed(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    string Reason);
