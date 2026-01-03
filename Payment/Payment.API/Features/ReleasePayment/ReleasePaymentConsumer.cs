using MassTransit;
using Payment.Application.Abstractions;
using Payment.Contracts.Events;
using Payment.Domain.Entities;
using ReleasePaymentCommand = Payment.Contracts.Commands.ReleasePayment;

namespace Payment.API.Features.ReleasePayment;

/// <summary>
/// Consumer that handles payment release commands.
/// </summary>
public class ReleasePaymentConsumer : IConsumer<ReleasePaymentCommand>
{
    private readonly IPaymentTransactionRepository _repository;

    public ReleasePaymentConsumer(IPaymentTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<ReleasePaymentCommand> context)
    {
        var command = context.Message;

        var transaction = await _repository.GetByIdAsync(command.PaymentAuthorisationId, context.CancellationToken);

        var utcNow = DateTime.UtcNow;

        if (transaction is not null && transaction.Status == PaymentStatus.Authorised)
        {
            transaction.Status = PaymentStatus.Released;
            transaction.ReleasedAt = utcNow;
            transaction.FailureReason = command.Reason;
            await _repository.UpdateAsync(transaction, context.CancellationToken);
        }

        await context.Publish(new PaymentReleased(
            command.CorrelationId,
            command.TripId,
            command.PaymentAuthorisationId,
            utcNow,
            command.Reason));
    }
}
