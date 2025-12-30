namespace Payment.Contracts.Commands;

public record RefundPayment(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentId,
    decimal Amount,
    string Reason);
