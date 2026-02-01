using MediatR;

namespace Trip.API.Features.ListTrips;

/// <summary>
/// Query for retrieving a paginated list of trips with optional filtering.
/// </summary>
public record ListTripsQuery(
    string? Status,
    Guid? CustomerId,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedTripsResponse>;
