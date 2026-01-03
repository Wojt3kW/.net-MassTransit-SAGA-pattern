using Insurance.Application.Abstractions;
using Insurance.Contracts.Events;
using Insurance.Domain.Entities;
using MassTransit;
using CancelInsuranceCommand = Insurance.Contracts.Commands.CancelInsurance;

namespace Insurance.API.Features.CancelInsurance;

/// <summary>
/// Consumer that handles insurance policy cancellation commands.
/// </summary>
public class CancelInsuranceConsumer : IConsumer<CancelInsuranceCommand>
{
    private readonly IInsurancePolicyRepository _repository;

    public CancelInsuranceConsumer(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CancelInsuranceCommand> context)
    {
        var command = context.Message;

        var policy = await _repository.GetByIdAsync(command.InsurancePolicyId, context.CancellationToken);

        var utcNow = DateTime.UtcNow;

        if (policy is not null)
        {
            policy.Status = InsurancePolicyStatus.Cancelled;
            policy.CancellationReason = command.Reason;
            policy.CancelledAt = utcNow;

            await _repository.UpdateAsync(policy, context.CancellationToken);
        }

        await context.Publish(new InsuranceCancelled(
            command.CorrelationId,
            command.TripId,
            command.InsurancePolicyId,
            utcNow,
            command.Reason));
    }
}
