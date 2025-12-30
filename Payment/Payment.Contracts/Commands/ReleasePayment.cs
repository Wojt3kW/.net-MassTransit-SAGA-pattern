namespace Payment.Contracts.Commands;

public record ReleasePayment(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    string Reason);
