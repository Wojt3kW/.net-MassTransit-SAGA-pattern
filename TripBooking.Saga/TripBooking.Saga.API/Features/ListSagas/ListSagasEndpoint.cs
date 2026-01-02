using MediatR;

namespace TripBooking.Saga.API.Features.ListSagas;

/// <summary>
/// Endpoint for retrieving a paginated list of sagas with filtering.
/// </summary>
public class ListSagasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sagas", async (
            string? state,
            Guid? customerId,
            int page,
            int pageSize,
            IMediator mediator) =>
        {
            var query = new ListSagasQuery(state, customerId, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("ListSagas")
        .WithTags("Saga Monitoring")
        .Produces<PagedSagaResponse>()
        .WithSummary("List all sagas with optional filtering")
        .WithDescription("Retrieves a paginated list of saga states with optional filtering by state and customer ID.");
    }
}
