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
            TripId: trip.Id,
            CustomerId: trip.CustomerId,
            CustomerEmail: trip.CustomerEmail,
            CustomerName: trip.CustomerName,
            Status: trip.Status.ToString(),
            OutboundFlightStatus: trip.OutboundFlightConfirmation is not null ? "Confirmed" : "Pending",
            ReturnFlightStatus: trip.ReturnFlightConfirmation is not null ? "Confirmed" : "Pending",
            HotelStatus: trip.HotelConfirmation is not null ? "Confirmed" : "Pending",
            GroundTransportStatus: trip.GroundTransportConfirmation is not null ? "Confirmed" : "NotRequested",
            InsuranceStatus: trip.InsurancePolicyNumber is not null ? "Issued" : "Pending",
            PaymentStatus: trip.PaymentConfirmation is not null ? "Captured" : "Pending",
            CreatedAt: trip.CreatedAt,
            CompletedAt: trip.CompletedAt,
            FailureReason: trip.FailureReason,
            TotalAmount: trip.TotalAmount);
    }
}
