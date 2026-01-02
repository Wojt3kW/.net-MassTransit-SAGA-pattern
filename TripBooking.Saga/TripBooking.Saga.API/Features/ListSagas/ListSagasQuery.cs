using MediatR;

namespace TripBooking.Saga.API.Features.ListSagas;

/// <summary>
/// Query to retrieve a paginated list of sagas with optional filtering.
/// </summary>
public record ListSagasQuery(
    string? State = null,
    Guid? CustomerId = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedSagaResponse>;
