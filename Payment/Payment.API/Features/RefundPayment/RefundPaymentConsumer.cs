using MassTransit;
using Payment.Application.Abstractions;
using Payment.Domain.Entities;
using Payment.Contracts.Events;
using RefundPaymentCommand = Payment.Contracts.Commands.RefundPayment;

namespace Payment.API.Features.RefundPayment;

/// <summary>
/// Consumer that handles payment refund commands.
/// </summary>
public class RefundPaymentConsumer : IConsumer<RefundPaymentCommand>
{
    private readonly IPaymentTransactionRepository _repository;

    public RefundPaymentConsumer(IPaymentTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<RefundPaymentCommand> context)
    {
        var command = context.Message;

        var transaction = await _repository.GetByIdAsync(command.PaymentId, context.CancellationToken);

        if (transaction is not null && transaction.Status == PaymentStatus.Captured)
        {
            transaction.Status = PaymentStatus.Refunded;
            transaction.RefundedAt = DateTime.UtcNow;
            transaction.FailureReason = command.Reason;
            await _repository.UpdateAsync(transaction, context.CancellationToken);
        }

        await context.Publish(new PaymentRefunded(
            command.CorrelationId,
            command.TripId,
            command.PaymentId,
            command.Amount,
            DateTime.UtcNow));
    }
}
