using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Payment.Contracts.Events;
using RefundPaymentCommand = Payment.Contracts.Commands.RefundPayment;

namespace Payment.API.Features.RefundPayment;

public class RefundPaymentConsumer : IConsumer<RefundPaymentCommand>
{
    private readonly PaymentDbContext _dbContext;

    public RefundPaymentConsumer(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<RefundPaymentCommand> context)
    {
        var command = context.Message;

        var transaction = await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(x => x.Id == command.PaymentId);

        if (transaction is not null && transaction.Status == PaymentStatus.Captured)
        {
            transaction.Status = PaymentStatus.Refunded;
            transaction.RefundedAt = DateTime.UtcNow;
            transaction.FailureReason = command.Reason;
            await _dbContext.SaveChangesAsync();
        }

        await context.Publish(new PaymentRefunded(
            command.CorrelationId,
            command.TripId,
            command.PaymentId,
            command.Amount,
            DateTime.UtcNow));
    }
}
