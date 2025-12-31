using Insurance.Application.Abstractions;

namespace Insurance.API.Features.DeleteInsurancePolicy;

/// <summary>
/// Endpoint to delete an insurance policy.
/// </summary>
public class DeleteInsurancePolicyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/insurance-policies/{id:guid}", async (
            Guid id,
            IInsurancePolicyRepository repository,
            CancellationToken cancellationToken) =>
        {
            var deleted = await repository.DeleteAsync(id, cancellationToken);

            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteInsurancePolicy")
        .WithTags("InsurancePolicies")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
