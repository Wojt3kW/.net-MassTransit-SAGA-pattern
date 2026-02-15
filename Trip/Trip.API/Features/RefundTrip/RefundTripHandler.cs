using MassTransit;
using MediatR;
using Trip.Application.Abstractions;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

namespace Trip.API.Features.RefundTrip;

/// <summary>Handles refund requests for completed trips.</summary>
public class RefundTripHandler : IRequestHandler<RefundTripCommand, bool>
{
    private readonly ITripRepository _tripRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public RefundTripHandler(ITripRepository tripRepository, IPublishEndpoint publishEndpoint)
    {
        _tripRepository = tripRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(RefundTripCommand request, CancellationToken cancellationToken)
    {
        var trip = await _tripRepository.GetByIdAsync(request.TripId, cancellationToken);

        if (trip is null)
            return false;

        // Only completed trips can be refunded
        if (trip.Status != TripStatus.Completed)
            return false;

        // Publish refund request event to trigger SAGA
        await _publishEndpoint.Publish(
            new TripRefundRequested(
                TripId: request.TripId,
                Reason: request.Reason,
                RequestedAt: DateTime.UtcNow),
            cancellationToken);

        return true;
    }
}
