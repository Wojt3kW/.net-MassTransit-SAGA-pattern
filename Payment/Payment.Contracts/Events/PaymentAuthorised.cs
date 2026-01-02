namespace Payment.Contracts.Events;

/// <summary>Published when payment has been successfully authorised (funds blocked on card).</summary>
public record PaymentAuthorised(
    Guid CorrelationId,
    Guid TripId,
    Guid PaymentAuthorisationId,
    string AuthorisationCode,
    decimal Amount,
    string Currency,
    DateTime AuthorisedAt);
