using MassTransit;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;
using Payment.Contracts.Events;
using AuthorisePaymentCommand = Payment.Contracts.Commands.AuthorisePayment;

namespace Payment.API.Features.AuthorisePayment;

public class AuthorisePaymentConsumer : IConsumer<AuthorisePaymentCommand>
{
    private readonly PaymentDbContext _dbContext;

    public AuthorisePaymentConsumer(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AuthorisePaymentCommand> context)
    {
        var command = context.Message;

        // Simulate payment authorisation (in real scenario, call payment gateway)
        var transaction = new PaymentTransaction
        {
            Id = Guid.NewGuid(),
            TripId = command.TripId,
            CustomerId = command.CustomerId,
            CardLastFourDigits = command.CardNumber[^4..],
            CardHolderName = command.CardHolderName,
            Amount = command.Amount,
            Currency = command.Currency,
            AuthorisationCode = GenerateAuthorisationCode(),
            Status = PaymentStatus.Authorised,
            CreatedAt = DateTime.UtcNow,
            AuthorisedAt = DateTime.UtcNow
        };

        _dbContext.PaymentTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

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
