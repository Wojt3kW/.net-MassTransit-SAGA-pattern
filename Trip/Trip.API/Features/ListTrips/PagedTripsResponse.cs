using Trip.Contracts.DTOs;

namespace Trip.API.Features.ListTrips;

/// <summary>
/// Paginated response containing trip booking statuses.
/// </summary>
public record PagedTripsResponse(
    IReadOnlyList<TripBookingStatusResponse> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
