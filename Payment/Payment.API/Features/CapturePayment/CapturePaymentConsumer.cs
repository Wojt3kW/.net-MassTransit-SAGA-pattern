using MassTransit;
using Payment.Application.Abstractions;
using Payment.Contracts.Events;
using Payment.Domain.Entities;
using CapturePaymentCommand = Payment.Contracts.Commands.CapturePayment;

namespace Payment.API.Features.CapturePayment;

/// <summary>
/// Consumer that handles payment capture commands.
/// </summary>
public class CapturePaymentConsumer : IConsumer<CapturePaymentCommand>
{
    private readonly IPaymentTransactionRepository _repository;

    public CapturePaymentConsumer(IPaymentTransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CapturePaymentCommand> context)
    {
        var command = context.Message;

        // SIMULATION: If amount is 999.99 - simulate capture failure
        if (command.Amount == 999.99m)
        {
            await context.Publish(new PaymentCaptureFailed(
                command.CorrelationId,
                command.TripId,
                command.PaymentAuthorisationId,
                "Simulated: Insufficient funds for capture"));
            return;
        }

        // SIMULATION: If amount is 888.88 - simulate timeout
        if (command.Amount == 888.88m)
        {
            await Task.Delay(TimeSpan.FromSeconds(35), context.CancellationToken);
        }

        var transaction = await _repository.GetByIdAsync(command.PaymentAuthorisationId, context.CancellationToken);

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
        await _repository.UpdateAsync(transaction, context.CancellationToken);

        await context.Publish(new PaymentCaptured(
            command.CorrelationId,
            command.TripId,
            transaction.Id,
            command.Amount,
            transaction.CapturedAt!.Value));
    }
}
