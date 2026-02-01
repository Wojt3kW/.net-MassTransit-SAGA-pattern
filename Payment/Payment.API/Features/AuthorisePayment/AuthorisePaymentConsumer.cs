using MassTransit;
using Payment.Application.Abstractions;
using Payment.Contracts.Events;
using Payment.Domain.Entities;
using AuthorisePaymentCommand = Payment.Contracts.Commands.AuthorisePayment;

namespace Payment.API.Features.AuthorisePayment;

/// <summary>
/// Consumes AuthorisePayment commands and processes payment authorisation.
/// </summary>
public class AuthorisePaymentConsumer : IConsumer<AuthorisePaymentCommand>
{
    private readonly IPaymentTransactionRepository _transactionRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public AuthorisePaymentConsumer(
        IPaymentTransactionRepository transactionRepository,
        IPaymentMethodRepository paymentMethodRepository)
    {
        _transactionRepository = transactionRepository;
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task Consume(ConsumeContext<AuthorisePaymentCommand> context)
    {
        var command = context.Message;

        // SIMULATION: If amount is 0.01 - simulate authorisation failure
        if (command.Amount == 0.01m)
        {
            await context.Publish(new PaymentAuthorisationFailed(
                command.CorrelationId,
                command.TripId,
                "Simulated: Card declined by issuer"));
            return;
        }

        // SIMULATION: If amount is 1_000_000_000 - simulate slow payment gateway (timeout test)
        // This will delay response for 35 seconds, causing PaymentAuthorisationTimedOut
        if (command.Amount == 1_000_000_000m)
        {
            await Task.Delay(TimeSpan.FromSeconds(35), context.CancellationToken);
        }

        // Get payment method - either by ID or customer's default
        var paymentMethod = command.PaymentMethodId.HasValue
            ? await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId.Value, context.CancellationToken)
            : await _paymentMethodRepository.GetDefaultByCustomerIdAsync(command.CustomerId, context.CancellationToken);

        if (paymentMethod is null)
        {
            await context.Publish(new PaymentAuthorisationFailed(
                command.CorrelationId,
                command.TripId,
                "No payment method found for customer"));
            return;
        }

        // Simulate payment authorisation (in real scenario, call payment gateway)
        await Task.Delay(TimeSpan.FromSeconds(5));

        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            CustomerId = command.CustomerId,
            CardLastFourDigits = paymentMethod.CardLastFourDigits,
            CardHolderName = paymentMethod.CardHolderName,
            Amount = command.Amount,
            Currency = command.Currency,
            AuthorisationCode = GenerateAuthorisationCode(),
            Status = PaymentStatus.Authorised,
            CreatedAt = DateTime.UtcNow,
            AuthorisedAt = DateTime.UtcNow
        };

        await _transactionRepository.AddAsync(transaction, context.CancellationToken);

        await context.Publish(new PaymentAuthorised(
            command.CorrelationId,
            command.TripId,
            transaction.Id,
            transaction.AuthorisationCode!,
            transaction.Amount,
            transaction.Currency,
            transaction.AuthorisedAt!.Value));
    }

    private static string GenerateAuthorisationCode() => $"AUTH-{Guid.NewGuid().ToString()[..8].ToUpper()}";
}
