using FlightBooking.Application.Abstractions;
using FlightBooking.Contracts.DTOs;
using FlightBooking.Domain.Entities;

namespace FlightBooking.API.Features.GetFlightReservations;

/// <summary>
/// Endpoint to get all flight reservations with optional filtering.
/// </summary>
public class GetFlightReservationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/flight-reservations", async (
            IFlightReservationRepository repository,
            Guid? tripId = null,
            string? status = null,
            string? type = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            ReservationStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ReservationStatus>(status, true, out var parsedStatus))
                statusEnum = parsedStatus;

            FlightType? typeEnum = null;
            if (!string.IsNullOrEmpty(type) && Enum.TryParse<FlightType>(type, true, out var parsedType))
                typeEnum = parsedType;

            var (items, totalCount) = await repository.GetAllAsync(
                tripId, statusEnum, typeEnum, page, pageSize, cancellationToken);

            var responses = items.Select(r => new FlightReservationResponse(
                r.Id,
                r.TripId,
                r.Type.ToString(),
                r.FlightNumber,
                r.Carrier,
                r.Origin,
                r.Destination,
                r.DepartureDate,
                r.ConfirmationCode,
                r.Price,
                r.PassengerCount,
                r.Status.ToString(),
                r.CancellationReason,
                r.CreatedAt,
                r.ConfirmedAt,
                r.CancelledAt)).ToList();

            return Results.Ok(new FlightReservationListResponse(responses, totalCount, page, pageSize));
        })
        .WithName("GetFlightReservations")
        .WithTags("FlightReservations")
        .WithOpenApi()
        .Produces<FlightReservationListResponse>();
    }
}
