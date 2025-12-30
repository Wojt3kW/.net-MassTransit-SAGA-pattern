using Payment.Application.Abstractions;

namespace Payment.API.Features.DeletePaymentTransaction;

/// <summary>
/// Endpoint to delete a payment transaction.
/// </summary>
public class DeletePaymentTransactionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/payment-transactions/{id:guid}", async (
            Guid id,
            IPaymentTransactionRepository repository,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);

            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeletePaymentTransaction")
        .WithTags("PaymentTransactions")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
