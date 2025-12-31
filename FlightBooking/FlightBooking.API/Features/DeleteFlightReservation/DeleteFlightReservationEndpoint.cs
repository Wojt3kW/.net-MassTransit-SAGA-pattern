using FlightBooking.Application.Abstractions;

namespace FlightBooking.API.Features.DeleteFlightReservation;

/// <summary>
/// Endpoint to delete a flight reservation.
/// </summary>
public class DeleteFlightReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/flight-reservations/{id:guid}", async (
            Guid id,
            IFlightReservationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);

            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteFlightReservation")
        .WithTags("FlightReservations")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
