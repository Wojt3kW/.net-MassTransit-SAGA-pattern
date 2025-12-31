using GroundTransport.Application.Abstractions;
using GroundTransport.Contracts.DTOs;

namespace GroundTransport.API.Features.GetTransportReservation;

/// <summary>
/// Endpoint to get a single transport reservation by ID.
/// </summary>
public class GetTransportReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/transport-reservations/{id:guid}", async (
            Guid id,
            ITransportReservationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var reservation = await repository.GetByIdAsync(id, cancellationToken);

            if (reservation is null)
                return Results.NotFound();

            var response = new TransportReservationResponse(
                reservation.Id,
                reservation.TripId,
                reservation.Type.ToString(),
                reservation.PickupLocation,
                reservation.DropoffLocation,
                reservation.PickupDate,
                reservation.PassengerCount,
                reservation.ConfirmationCode,
                reservation.Price,
                reservation.Status.ToString(),
                reservation.CancellationReason,
                reservation.CreatedAt,
                reservation.ConfirmedAt,
                reservation.CancelledAt);

            return Results.Ok(response);
        })
        .WithName("GetTransportReservation")
        .WithTags("TransportReservations")
        .Produces<TransportReservationResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
