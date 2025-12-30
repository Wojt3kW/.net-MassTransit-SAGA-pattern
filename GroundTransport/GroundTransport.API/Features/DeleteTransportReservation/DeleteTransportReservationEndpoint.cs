using GroundTransport.Application.Abstractions;

namespace GroundTransport.API.Features.DeleteTransportReservation;

/// <summary>
/// Endpoint to delete a transport reservation.
/// </summary>
public class DeleteTransportReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/transport-reservations/{id:guid}", async (
            Guid id,
            ITransportReservationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);

            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTransportReservation")
        .WithTags("TransportReservations")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
