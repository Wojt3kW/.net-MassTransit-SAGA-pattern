using MediatR;
using Microsoft.EntityFrameworkCore;
using TripBooking.Saga.Persistence;

namespace TripBooking.Saga.API.Features.ListSagas;

/// <summary>
/// Handler for retrieving a paginated list of sagas.
/// </summary>
public class ListSagasHandler(TripBookingSagaDbContext dbContext) 
    : IRequestHandler<ListSagasQuery, PagedSagaResponse>
{
    public async Task<PagedSagaResponse> Handle(ListSagasQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.TripBookingSagaStates.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.State))
            query = query.Where(s => s.CurrentState == request.State);

        if (request.CustomerId.HasValue)
            query = query.Where(s => s.CustomerId == request.CustomerId.Value);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SagaSummaryResponse(
                s.CorrelationId,
                s.TripId,
                s.CurrentState,
                s.CustomerId,
                s.CustomerEmail,
                s.TotalAmount,
                s.CreatedAt,
                s.CompletedAt,
                s.FailureReason
            ))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PagedSagaResponse(items, request.Page, request.PageSize, totalCount, totalPages);
    }
}
