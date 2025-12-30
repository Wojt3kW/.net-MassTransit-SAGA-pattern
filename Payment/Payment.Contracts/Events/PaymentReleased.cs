namespace Payment.Contracts.Events;

public record PaymentReleased(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    DateTime ReleasedAt);
