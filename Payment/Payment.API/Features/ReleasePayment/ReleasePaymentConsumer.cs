using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Payment.Contracts.Events;
using ReleasePaymentCommand = Payment.Contracts.Commands.ReleasePayment;

namespace Payment.API.Features.ReleasePayment;

public class ReleasePaymentConsumer : IConsumer<ReleasePaymentCommand>
{
    private readonly PaymentDbContext _dbContext;

    public ReleasePaymentConsumer(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<ReleasePaymentCommand> context)
    {
        var command = context.Message;

        var transaction = await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(x => x.Id == command.PaymentAuthorisationId);

        if (transaction is not null && transaction.Status == PaymentStatus.Authorised)
        {
            transaction.Status = PaymentStatus.Released;
            transaction.ReleasedAt = DateTime.UtcNow;
            transaction.FailureReason = command.Reason;
            await _dbContext.SaveChangesAsync();
        }

        await context.Publish(new PaymentReleased(
            command.CorrelationId,
            command.TripId,
            command.PaymentAuthorisationId,
            DateTime.UtcNow));
    }
}
