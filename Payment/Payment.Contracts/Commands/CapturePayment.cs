namespace Payment.Contracts.Commands;

public record CapturePayment(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    decimal Amount);
