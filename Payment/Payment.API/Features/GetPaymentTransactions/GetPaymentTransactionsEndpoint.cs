using Payment.Application.Abstractions;
using Payment.Contracts.DTOs;
using Payment.Domain.Entities;

namespace Payment.API.Features.GetPaymentTransactions;

/// <summary>
/// Endpoint to get all payment transactions with optional filtering.
/// </summary>
public class GetPaymentTransactionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payment-transactions", async (
            IPaymentTransactionRepository repository,
            Guid? tripId = null,
            string? status = null,
            Guid? transactionId = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            PaymentStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<PaymentStatus>(status, true, out var parsed))
                statusEnum = parsed;

            var (items, totalCount) = await repository.GetAllAsync(
                tripId,
                statusEnum,
                transactionId,
                page,
                pageSize,
                cancellationToken);

            var response = items.Select(t => new PaymentTransactionResponse(
                t.Id,
                t.TripId,
                t.CustomerId,
                t.CardLastFourDigits,
                t.CardHolderName,
                t.Amount,
                t.Currency,
                t.AuthorisationCode,
                t.Status.ToString(),
                t.FailureReason,
                t.CreatedAt,
                t.AuthorisedAt,
                t.CapturedAt,
                t.ReleasedAt,
                t.RefundedAt)).ToList();

            return Results.Ok(new PaymentTransactionListResponse(response, totalCount, page, pageSize));
        })
        .WithName("GetPaymentTransactions")
        .WithTags("PaymentTransactions")
        .Produces<PaymentTransactionListResponse>();
    }
}
