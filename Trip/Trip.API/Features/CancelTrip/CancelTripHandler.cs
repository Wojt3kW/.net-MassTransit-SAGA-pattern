using MassTransit;
using MediatR;
using Trip.Application.Abstractions;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

namespace Trip.API.Features.CancelTrip;

public class CancelTripHandler : IRequestHandler<CancelTripCommand, bool>
{
    private readonly ITripRepository _tripRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CancelTripHandler(ITripRepository tripRepository, IPublishEndpoint publishEndpoint)
    {
        _tripRepository = tripRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(CancelTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
            return false;

        if (trip.Status == TripStatus.Cancelled || trip.Status == TripStatus.Completed)
            return false;

        // Publish cancellation request to SAGA
        await _publishEndpoint.Publish(
            new TripBookingCancelled(request.TripId, request.Reason, DateTime.UtcNow),
            cancellationToken);

        return true;
    }
}
