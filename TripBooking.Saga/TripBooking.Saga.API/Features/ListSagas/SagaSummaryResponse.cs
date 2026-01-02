namespace TripBooking.Saga.API.Features.ListSagas;

/// <summary>
/// Summary information for a saga in the list view.
/// </summary>
public record SagaSummaryResponse(
    Guid CorrelationId,
    Guid TripId,
    string CurrentState,
    Guid CustomerId,
    string CustomerEmail,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    string? FailureReason
);

/// <summary>
/// Paginated response for saga list.
/// </summary>
public record PagedSagaResponse(
    IEnumerable<SagaSummaryResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
