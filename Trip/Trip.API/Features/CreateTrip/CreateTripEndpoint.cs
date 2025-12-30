using MediatR;
using Trip.API.Features;
using Trip.Contracts.DTOs;

namespace Trip.API.Features.CreateTrip;

public class CreateTripEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/trips", async (CreateTripCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/trips/{result.TripId}", result);
        })
        .WithName("CreateTrip")
        .WithOpenApi()
        .Produces<TripBookingResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
