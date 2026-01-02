using MassTransit;
using Payment.Application.Abstractions;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Payment.Contracts.Events;
using AuthorisePaymentCommand = Payment.Contracts.Commands.AuthorisePayment;

namespace Payment.API.Features.AuthorisePayment;

/// <summary>
/// Consumes AuthorisePayment commands and processes payment authorisation.
/// </summary>
public class AuthorisePaymentConsumer : IConsumer<AuthorisePaymentCommand>
{
    private readonly PaymentDbContext _dbContext;
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public AuthorisePaymentConsumer(PaymentDbContext dbContext, IPaymentMethodRepository paymentMethodRepository)
    {
        _dbContext = dbContext;
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task Consume(ConsumeContext<AuthorisePaymentCommand> context)
    {
        var command = context.Message;

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

        _dbContext.PaymentTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

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
