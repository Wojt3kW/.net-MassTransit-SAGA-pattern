using Microsoft.EntityFrameworkCore;
using TripBooking.Saga.API.Features.ListSagas;
using TripBooking.Saga.Persistence;
using TripBooking.Saga.States;

namespace TripBooking.Saga.API.Features.StreamSagas;

/// <summary>
/// SSE endpoint for real-time streaming of saga state changes.
/// </summary>
public class StreamSagasEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sagas/stream", async (
            TripBookingSagaDbContext db,
            CancellationToken ct) =>
        {
            return TypedResults.ServerSentEvents(StreamSagaUpdates(db, ct));
        })
        .WithName("StreamSagas")
        .WithTags("Saga Monitoring")
        .WithSummary("Stream saga state updates via SSE")
        .WithDescription("Real-time Server-Sent Events stream of saga state changes. Sends updates every second.");
    }

    private static async IAsyncEnumerable<SagaSummaryResponse> StreamSagaUpdates(
        TripBookingSagaDbContext db,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        string? lastStatesHash = null;

        while (!ct.IsCancellationRequested)
        {
            var sagas = await db.Set<TripBookingSagaState>()
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .Take(100)
                .Select(s => new SagaSummaryResponse(
                    s.CorrelationId,
                    s.TripId,
                    s.CurrentState,
                    s.CustomerId,
                    s.CustomerEmail,
                    s.TotalAmount,
                    s.CreatedAt,
                    s.CompletedAt,
                    s.FailureReason
                ))
                .ToListAsync(ct);

            // Calculate hash to detect changes
            var currentHash = CalculateHash(sagas);

            // Only send data when state changes
            if (currentHash != lastStatesHash)
            {
                lastStatesHash = currentHash;
                foreach (var saga in sagas)
                {
                    yield return saga;
                }
            }

            await Task.Delay(1000, ct);
        }
    }

    private static string CalculateHash(IEnumerable<SagaSummaryResponse> sagas)
    {
        var combined = string.Join("|", sagas.Select(s => $"{s.CorrelationId}:{s.CurrentState}:{s.CompletedAt}"));
        return combined.GetHashCode().ToString();
    }
}
