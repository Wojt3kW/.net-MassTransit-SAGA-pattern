using Payment.Application.Abstractions;
using Payment.Contracts.DTOs;

namespace Payment.API.Features.GetPaymentTransaction;

/// <summary>
/// Endpoint to get a single payment transaction by ID.
/// </summary>
public class GetPaymentTransactionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payment-transactions/{id:guid}", async (
            Guid id,
            IPaymentTransactionRepository repository,
            CancellationToken cancellationToken) =>
        {
            var transaction = await repository.GetByIdAsync(id, cancellationToken);

            if (transaction is null)
                return Results.NotFound();

            var response = new PaymentTransactionResponse(
                transaction.Id,
                transaction.TripId,
                transaction.CustomerId,
                transaction.CardLastFourDigits,
                transaction.CardHolderName,
                transaction.Amount,
                transaction.Currency,
                transaction.AuthorisationCode,
                transaction.Status.ToString(),
                transaction.FailureReason,
                transaction.CreatedAt,
                transaction.AuthorisedAt,
                transaction.CapturedAt,
                transaction.ReleasedAt,
                transaction.RefundedAt);

            return Results.Ok(response);
        })
        .WithName("GetPaymentTransaction")
        .WithTags("PaymentTransactions")
        .Produces<PaymentTransactionResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
