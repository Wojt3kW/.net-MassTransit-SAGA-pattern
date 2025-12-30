namespace Payment.Contracts.Events;

public record PaymentAuthorisationFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
