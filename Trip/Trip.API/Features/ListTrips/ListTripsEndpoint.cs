using MediatR;

namespace Trip.API.Features.ListTrips;

/// <summary>
/// Endpoint for retrieving a paginated list of trips with filtering.
/// </summary>
public class ListTripsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/trips", async (
            string? status,
            Guid? customerId,
            int page = 1,
            int pageSize = 10,
            IMediator mediator = default!) =>
        {
            var query = new ListTripsQuery(status, customerId, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("ListTrips")
        .Produces<PagedTripsResponse>()
        .WithSummary("List all trips with optional filtering")
        .WithDescription("Retrieves a paginated list of trip bookings with optional filtering by status and customer ID.");
    }
}
