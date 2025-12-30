using FlightBooking.Application.Abstractions;
using FlightBooking.Contracts.DTOs;

namespace FlightBooking.API.Features.GetFlightReservation;

/// <summary>
/// Endpoint to get a single flight reservation by ID.
/// </summary>
public class GetFlightReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/flight-reservations/{id:guid}", async (
            Guid id,
            IFlightReservationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var reservation = await repository.GetByIdAsync(id, cancellationToken);

            if (reservation is null)
                return Results.NotFound();

            var response = new FlightReservationResponse(
                reservation.Id,
                reservation.TripId,
                reservation.Type.ToString(),
                reservation.FlightNumber,
                reservation.Carrier,
                reservation.Origin,
                reservation.Destination,
                reservation.DepartureDate,
                reservation.ConfirmationCode,
                reservation.Price,
                reservation.PassengerCount,
                reservation.Status.ToString(),
                reservation.CancellationReason,
                reservation.CreatedAt,
                reservation.ConfirmedAt,
                reservation.CancelledAt);

            return Results.Ok(response);
        })
        .WithName("GetFlightReservation")
        .WithTags("FlightReservations")
        .WithOpenApi()
        .Produces<FlightReservationResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
