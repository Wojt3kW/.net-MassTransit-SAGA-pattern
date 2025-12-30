using Insurance.Domain.Entities;

namespace Insurance.Application.Abstractions;

/// <summary>
/// Repository interface for insurance policy data access operations.
/// </summary>
public interface IInsurancePolicyRepository
{
    /// <summary>Gets an insurance policy by its unique identifier.</summary>
    Task<InsurancePolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets all insurance policies with optional filtering and pagination.</summary>
    Task<(IReadOnlyList<InsurancePolicy> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        InsurancePolicyStatus? status = null,
        string? policyNumber = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new insurance policy to the repository.</summary>
    Task<InsurancePolicy> AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing insurance policy.</summary>
    Task UpdateAsync(InsurancePolicy policy, CancellationToken cancellationToken = default);

    /// <summary>Deletes an insurance policy by its unique identifier.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets an insurance policy by trip identifier.</summary>
    Task<InsurancePolicy?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>Gets an insurance policy by policy number.</summary>
    Task<InsurancePolicy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default);
}
