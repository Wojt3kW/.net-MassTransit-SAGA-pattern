namespace Payment.Contracts.DTOs;

/// <summary>
/// Response DTO for payment transaction details.
/// </summary>
public record PaymentTransactionResponse(
    Guid Id,
    Guid TripId,
    Guid CustomerId,
    string CardLastFourDigits,
    string CardHolderName,
    decimal Amount,
    string Currency,
    string? AuthorisationCode,
    string Status,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime? AuthorisedAt,
    DateTime? CapturedAt,
    DateTime? ReleasedAt,
    DateTime? RefundedAt);
