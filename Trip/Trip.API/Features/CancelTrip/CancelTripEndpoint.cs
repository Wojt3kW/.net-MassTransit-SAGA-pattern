using MediatR;

namespace Trip.API.Features.CancelTrip;

public class CancelTripEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/trips/{tripId:guid}/cancel", async (Guid tripId, CancelTripRequest request, IMediator mediator) =>
        {
            var result = await mediator.Send(new CancelTripCommand(tripId, request.Reason));
            return result ? Results.Accepted() : Results.NotFound();
        })
        .WithName("CancelTrip")
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status404NotFound);
    }
}
