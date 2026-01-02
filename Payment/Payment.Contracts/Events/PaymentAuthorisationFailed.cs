namespace Payment.Contracts.Events;

/// <summary>Published when payment authorisation has failed (e.g., insufficient funds, card declined).</summary>
public record PaymentAuthorisationFailed(
    Guid CorrelationId,
    Guid TripId,
    string Reason);
