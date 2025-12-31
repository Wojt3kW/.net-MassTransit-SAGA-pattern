using HotelBooking.Application.Abstractions;
using HotelBooking.Contracts.DTOs;

namespace HotelBooking.API.Features.GetHotelReservation;

/// <summary>
/// Endpoint to get a single hotel reservation by ID.
/// </summary>
public class GetHotelReservationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotel-reservations/{id:guid}", async (
            Guid id,
            IHotelReservationRepository repository,
            CancellationToken cancellationToken) =>
        {
            var reservation = await repository.GetByIdAsync(id, cancellationToken);

            if (reservation is null)
                return Results.NotFound();

            var response = new HotelReservationResponse(
                reservation.Id,
                reservation.TripId,
                reservation.HotelId,
                reservation.HotelName,
                reservation.CheckIn,
                reservation.CheckOut,
                reservation.NumberOfGuests,
                reservation.GuestName,
                reservation.GuestEmail,
                reservation.ConfirmationCode,
                reservation.PricePerNight,
                reservation.TotalPrice,
                reservation.Status.ToString(),
                reservation.CancellationReason,
                reservation.CreatedAt,
                reservation.ConfirmedAt,
                reservation.CancelledAt,
                reservation.ExpiresAt);

            return Results.Ok(response);
        })
        .WithName("GetHotelReservation")
        .WithTags("HotelReservations")
        .Produces<HotelReservationResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
