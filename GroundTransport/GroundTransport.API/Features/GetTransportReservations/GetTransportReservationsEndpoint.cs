using GroundTransport.Application.Abstractions;
using GroundTransport.Contracts.DTOs;
using GroundTransport.Domain.Entities;

namespace GroundTransport.API.Features.GetTransportReservations;

/// <summary>
/// Endpoint to get all transport reservations with optional filtering.
/// </summary>
public class GetTransportReservationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/transport-reservations", async (
            ITransportReservationRepository repository,
            Guid? tripId = null,
            string? status = null,
            string? type = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            TransportReservationStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TransportReservationStatus>(status, true, out var parsedStatus))
                statusEnum = parsedStatus;

            TransportType? typeEnum = null;
            if (!string.IsNullOrEmpty(type) && Enum.TryParse<TransportType>(type, true, out var parsedType))
                typeEnum = parsedType;

            var (items, totalCount) = await repository.GetAllAsync(
                tripId, statusEnum, typeEnum, page, pageSize, cancellationToken);

            var response = items.Select(r => new TransportReservationResponse(
                r.Id,
                r.TripId,
                r.Type.ToString(),
                r.PickupLocation,
                r.DropoffLocation,
                r.PickupDate,
                r.PassengerCount,
                r.ConfirmationCode,
                r.Price,
                r.Status.ToString(),
                r.CancellationReason,
                r.CreatedAt,
                r.ConfirmedAt,
                r.CancelledAt)).ToList();

            return Results.Ok(new TransportReservationListResponse(response, totalCount, page, pageSize));
        })
        .WithName("GetTransportReservations")
        .WithTags("TransportReservations")
        .WithOpenApi()
        .Produces<TransportReservationListResponse>();
    }
}
