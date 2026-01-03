using MassTransit;
using Trip.Application.Abstractions;
using Trip.Contracts.Events;
using Trip.Domain.Entities;

namespace Trip.API.Consumers;

/// <summary>
/// Handles TripBookingCompleted events to update trip status to Completed.
/// </summary>
public class TripBookingCompletedConsumer : IConsumer<TripBookingCompleted>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<TripBookingCompletedConsumer> _logger;

    public TripBookingCompletedConsumer(
        ITripRepository tripRepository,
        ILogger<TripBookingCompletedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TripBookingCompleted> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received TripBookingCompleted for TripId: {TripId}, CompletedAt: {CompletedAt}",
            message.TripId,
            message.CompletedAt);

        var updated = await _tripRepository.UpdateStatusAsync(
            message.TripId,
            TripStatus.Completed,
            message.CompletedAt,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Trip {TripId} marked as Completed at {CompletedAt}",
            message.TripId,
            message.CompletedAt);
    }
}
