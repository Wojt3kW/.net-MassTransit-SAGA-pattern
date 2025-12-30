using MediatR;
using Trip.Application.Abstractions;
using Trip.Contracts.DTOs;

namespace Trip.API.Features.GetTrip;

public class GetTripHandler : IRequestHandler<GetTripQuery, TripBookingStatusResponse?>
{
    private readonly ITripRepository _tripRepository;

    public GetTripHandler(ITripRepository tripRepository)
    {
        _tripRepository = tripRepository;
    }

    public async Task<TripBookingStatusResponse?> Handle(GetTripQuery request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
            return null;

        return new TripBookingStatusResponse(
            trip.Id,
            trip.Status.ToString(),
            trip.OutboundFlightConfirmation is not null ? "Confirmed" : "Pending",
            trip.ReturnFlightConfirmation is not null ? "Confirmed" : "Pending",
            trip.HotelConfirmation is not null ? "Confirmed" : "Pending",
            trip.GroundTransportConfirmation is not null ? "Confirmed" : "NotRequested",
            trip.InsurancePolicyNumber is not null ? "Issued" : "Pending",
            trip.PaymentConfirmation is not null ? "Captured" : "Pending",
            trip.CreatedAt,
            trip.CompletedAt,
            trip.FailureReason);
    }
}
