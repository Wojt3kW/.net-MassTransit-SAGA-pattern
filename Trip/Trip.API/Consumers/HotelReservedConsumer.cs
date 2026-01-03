using HotelBooking.Contracts.Events;
using MassTransit;
using Trip.Application.Abstractions;

namespace Trip.API.Consumers;

/// <summary>
/// Handles HotelReserved events to update trip booking with hotel confirmation code.
/// </summary>
public class HotelReservedConsumer : IConsumer<HotelReserved>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<HotelReservedConsumer> _logger;

    public HotelReservedConsumer(
        ITripRepository tripRepository,
        ILogger<HotelReservedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<HotelReserved> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received HotelReserved for TripId: {TripId}, ConfirmationCode: {ConfirmationCode}",
            message.TripId,
            message.ConfirmationCode);

        var updated = await _tripRepository.UpdateHotelConfirmationAsync(
            message.TripId,
            message.ConfirmationCode,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Updated TripId: {TripId} with HotelConfirmation: {ConfirmationCode}",
            message.TripId,
            message.ConfirmationCode);
    }
}
