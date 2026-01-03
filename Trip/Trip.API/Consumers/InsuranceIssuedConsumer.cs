using Insurance.Contracts.Events;
using MassTransit;
using Trip.Application.Abstractions;

namespace Trip.API.Consumers;

/// <summary>
/// Handles InsuranceIssued events to update trip booking with insurance policy number.
/// </summary>
public class InsuranceIssuedConsumer : IConsumer<InsuranceIssued>
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<InsuranceIssuedConsumer> _logger;

    public InsuranceIssuedConsumer(
        ITripRepository tripRepository,
        ILogger<InsuranceIssuedConsumer> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InsuranceIssued> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Received InsuranceIssued for TripId: {TripId}, PolicyNumber: {PolicyNumber}",
            message.TripId,
            message.PolicyNumber);

        var updated = await _tripRepository.UpdateInsurancePolicyNumberAsync(
            message.TripId,
            message.PolicyNumber,
            context.CancellationToken);

        if (!updated)
        {
            _logger.LogWarning(
                "Trip not found: {TripId}. Will retry.",
                message.TripId);
            throw new InvalidOperationException($"Trip {message.TripId} not found. Retrying...");
        }

        _logger.LogInformation(
            "Updated TripId: {TripId} with InsurancePolicyNumber: {PolicyNumber}",
            message.TripId,
            message.PolicyNumber);
    }
}
