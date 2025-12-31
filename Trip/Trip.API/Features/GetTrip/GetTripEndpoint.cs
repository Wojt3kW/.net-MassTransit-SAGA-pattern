using MediatR;
using Trip.API.Features;
using Trip.Contracts.DTOs;

namespace Trip.API.Features.GetTrip;

public class GetTripEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/trips/{tripId:guid}", async (Guid tripId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTripQuery(tripId));
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetTrip")
        .Produces<TripBookingStatusResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
