using HotelBooking.Application.Abstractions;

namespace HotelBooking.API.Features.DeleteHotelReservation;

/// <summary>
/// Endpoint to delete a hotel reservation.
/// </summary>
public class DeleteHotelReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/hotel-reservations/{id:guid}", async (
            Guid id,
            IHotelReservationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteHotelReservation")
        .WithTags("HotelReservations")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
