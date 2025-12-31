using Insurance.Application.Abstractions;
using Insurance.Contracts.DTOs;
using Insurance.Domain.Entities;

namespace Insurance.API.Features.GetInsurancePolicies;

/// <summary>
/// Endpoint to get all insurance policies with optional filtering.
/// </summary>
public class GetInsurancePoliciesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/insurance-policies", async (
            IInsurancePolicyRepository repository,
            Guid? tripId = null,
            string? status = null,
            string? policyNumber = null,
            int page = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            InsurancePolicyStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<InsurancePolicyStatus>(status, true, out var parsedStatus))
                statusEnum = parsedStatus;

            var (items, totalCount) = await repository.GetAllAsync(
                tripId,
                statusEnum,
                policyNumber,
                page,
                pageSize,
                cancellationToken);

            var response = items.Select(p => new InsurancePolicyResponse(
                p.Id,
                p.TripId,
                p.CustomerId,
                p.CustomerName,
                p.CustomerEmail,
                p.PolicyNumber,
                p.CoverageStartDate,
                p.CoverageEndDate,
                p.OutboundFlightReservationId,
                p.ReturnFlightReservationId,
                p.HotelReservationId,
                p.TripTotalValue,
                p.Premium,
                p.Status.ToString(),
                p.CancellationReason,
                p.CreatedAt,
                p.IssuedAt,
                p.CancelledAt)).ToList();

            return Results.Ok(new InsurancePolicyListResponse(response, totalCount, page, pageSize));
        })
        .WithName("GetInsurancePolicies")
        .WithTags("InsurancePolicies")
        .Produces<InsurancePolicyListResponse>();
    }
}
