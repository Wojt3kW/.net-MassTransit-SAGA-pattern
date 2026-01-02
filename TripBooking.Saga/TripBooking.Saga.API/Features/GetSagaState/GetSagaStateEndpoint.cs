using MediatR;

namespace TripBooking.Saga.API.Features.GetSagaState;

/// <summary>
/// Endpoint for retrieving saga state by trip ID.
/// </summary>
public class GetSagaStateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sagas/{tripId:guid}", async (Guid tripId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetSagaStateQuery(tripId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetSagaState")
        .WithTags("Saga Monitoring")
        .Produces<SagaStateResponse>()
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get saga state by trip ID")
        .WithDescription("Retrieves the current state of a trip booking saga including all reservation IDs and flags.");
    }
}
