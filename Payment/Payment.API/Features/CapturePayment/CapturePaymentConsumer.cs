using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Payment.Contracts.Events;
using CapturePaymentCommand = Payment.Contracts.Commands.CapturePayment;

namespace Payment.API.Features.CapturePayment;

public class CapturePaymentConsumer : IConsumer<CapturePaymentCommand>
{
    private readonly PaymentDbContext _dbContext;

    public CapturePaymentConsumer(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CapturePaymentCommand> context)
    {
        var command = context.Message;

        var transaction = await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(x => x.Id == command.PaymentAuthorisationId);

        if (transaction is null || transaction.Status != PaymentStatus.Authorised)
        {
            await context.Publish(new PaymentCaptureFailed(
                command.CorrelationId,
                command.TripId,
                command.PaymentAuthorisationId,
                "Payment not found or not in authorised state"));
            return;
        }

        transaction.Status = PaymentStatus.Captured;
        transaction.CapturedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        await context.Publish(new PaymentCaptured(
            command.CorrelationId,
            command.TripId,
            transaction.Id,
            command.Amount,
            transaction.CapturedAt!.Value));
    }
}
