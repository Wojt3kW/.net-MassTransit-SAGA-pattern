using Insurance.Domain.Entities;
using Insurance.Infrastructure.Persistence;
using Insurance.Contracts.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using CancelInsuranceCommand = Insurance.Contracts.Commands.CancelInsurance;

namespace Insurance.API.Features.CancelInsurance;

public class CancelInsuranceConsumer : IConsumer<CancelInsuranceCommand>
{
    private readonly InsuranceDbContext _dbContext;

    public CancelInsuranceConsumer(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CancelInsuranceCommand> context)
    {
        var command = context.Message;

        var policy = await _dbContext.InsurancePolicies
            .FirstOrDefaultAsync(x => x.Id == command.InsurancePolicyId);

        if (policy is not null)
        {
            policy.Status = InsurancePolicyStatus.Cancelled;
            policy.CancellationReason = command.Reason;
            policy.CancelledAt = DateTime.UtcNow;
            
            await _dbContext.SaveChangesAsync();
        }

        await context.Publish(new InsuranceCancelled(
            command.CorrelationId,
            command.TripId,
            command.InsurancePolicyId,
            DateTime.UtcNow));
    }
}
