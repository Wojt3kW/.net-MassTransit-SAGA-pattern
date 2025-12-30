namespace Payment.Contracts.DTOs;

/// <summary>
/// Response DTO for paginated list of payment transactions.
/// </summary>
public record PaymentTransactionListResponse(
    IReadOnlyList<PaymentTransactionResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
