using MassTransit;
using Trip.Application.Abstractions;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

namespace Trip.API.Consumers;

/// <summary>
/// Handles TripBookingCancelled events to update trip status to Cancelled.
/// </summary>
public class TripBookingCancelledConsumer : IConsumer<TripBookingCancelled>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<TripBookingCancelledConsumer> _logger;

    public TripBookingCancelledConsumer(
        ITripRepository tripRepository,
        ILogger<TripBookingCancelledConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TripBookingCancelled> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received TripBookingCancelled for TripId: {TripId}, Reason: {Reason}, CancelledAt: {CancelledAt}",
            message.TripId,
            message.Reason,
            message.CancelledAt);

        var updated = await _tripRepository.UpdateStatusAsync(
            message.TripId,
            TripStatus.Cancelled,
            message.CancelledAt,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Trip {TripId} marked as Cancelled at {CancelledAt}. Reason: {Reason}",
            message.TripId,
            message.CancelledAt,
            message.Reason);
    }
}
