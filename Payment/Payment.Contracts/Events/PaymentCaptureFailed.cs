namespace Payment.Contracts.Events;

/// <summary>Published when payment capture has failed after authorisation.</summary>
public record PaymentCaptureFailed(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    string Reason);
