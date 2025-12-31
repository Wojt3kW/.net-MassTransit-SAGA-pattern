using HotelBooking.Application.Abstractions;
using HotelBooking.Contracts.DTOs;
using HotelBooking.Domain.Entities;

namespace HotelBooking.API.Features.GetHotelReservations;

/// <summary>
/// Endpoint to get all hotel reservations with optional filtering.
/// </summary>
public class GetHotelReservationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotel-reservations", async (
            IHotelReservationRepository repository,
            Guid? tripId = null,
            string? status = null,
            string? hotelId = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            HotelReservationStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<HotelReservationStatus>(status, true, out var parsed))
                statusEnum = parsed;

            var (items, totalCount) = await repository.GetAllAsync(
                tripId, statusEnum, hotelId, page, pageSize, cancellationToken);

            var response = items.Select(r => new HotelReservationResponse(
                r.Id,
                r.TripId,
                r.HotelId,
                r.HotelName,
                r.CheckIn,
                r.CheckOut,
                r.NumberOfGuests,
                r.GuestName,
                r.GuestEmail,
                r.ConfirmationCode,
                r.PricePerNight,
                r.TotalPrice,
                r.Status.ToString(),
                r.CancellationReason,
                r.CreatedAt,
                r.ConfirmedAt,
                r.CancelledAt,
                r.ExpiresAt)).ToList();

            return Results.Ok(new HotelReservationListResponse(response, totalCount, page, pageSize));
        })
        .WithName("GetHotelReservations")
        .WithTags("HotelReservations")
        .Produces<HotelReservationListResponse>();
    }
}
