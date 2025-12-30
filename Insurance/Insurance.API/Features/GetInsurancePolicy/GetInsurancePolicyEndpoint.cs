using Insurance.Application.Abstractions;
using Insurance.Contracts.DTOs;

namespace Insurance.API.Features.GetInsurancePolicy;

/// <summary>
/// Endpoint to get a single insurance policy by ID.
/// </summary>
public class GetInsurancePolicyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/insurance-policies/{id:guid}", async (
            Guid id,
            IInsurancePolicyRepository repository,
            CancellationToken cancellationToken) =>
        {
            var policy = await repository.GetByIdAsync(id, cancellationToken);

            if (policy is null)
                return Results.NotFound();

            var response = new InsurancePolicyResponse(
                policy.Id,
                policy.TripId,
                policy.CustomerId,
                policy.CustomerName,
                policy.CustomerEmail,
                policy.PolicyNumber,
                policy.CoverageStartDate,
                policy.CoverageEndDate,
                policy.OutboundFlightReservationId,
                policy.ReturnFlightReservationId,
                policy.HotelReservationId,
                policy.TripTotalValue,
                policy.Premium,
                policy.Status.ToString(),
                policy.CancellationReason,
                policy.CreatedAt,
                policy.IssuedAt,
                policy.CancelledAt);

            return Results.Ok(response);
        })
        .WithName("GetInsurancePolicy")
        .WithTags("InsurancePolicies")
        .WithOpenApi()
        .Produces<InsurancePolicyResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}
