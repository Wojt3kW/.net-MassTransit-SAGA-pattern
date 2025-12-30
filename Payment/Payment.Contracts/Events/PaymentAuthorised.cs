namespace Payment.Contracts.Events;

public record PaymentAuthorised(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    string AuthorisationCode,
    decimal Amount,
    string Currency,
    DateTime AuthorisedAt);
