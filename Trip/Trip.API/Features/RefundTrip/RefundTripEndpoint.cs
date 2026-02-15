using MediatR;

namespace Trip.API.Features.RefundTrip;

/// <summary>Endpoint for requesting trip refunds.</summary>
public class RefundTripEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/trips/{tripId:guid}/refund", async (Guid tripId, RefundTripRequest request, IMediator mediator) =>
        {
            var command = new RefundTripCommand(tripId, request.Reason);
            var result = await mediator.Send(command);
            return result ? Results.Accepted() : Results.NotFound();
        })
        .WithName("RefundTrip")
        .WithTags("Trips")
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Request a refund for a completed trip")
        .WithDescription("Initiates a refund process for a completed trip. Only trips with status 'Completed' can be refunded.");
    }
}
