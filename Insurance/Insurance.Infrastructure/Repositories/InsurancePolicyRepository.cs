using Insurance.Application.Abstractions;
using Insurance.Domain.Entities;
using Insurance.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for insurance policy data access operations.
/// </summary>
public class InsurancePolicyRepository : IInsurancePolicyRepository
{
    private readonly InsuranceDbContext _dbContext;

    /// <summary>Initializes a new instance of the repository.</summary>
    public InsurancePolicyRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<InsurancePolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InsurancePolicies
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<InsurancePolicy> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        InsurancePolicyStatus? status = null,
        string? policyNumber = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.InsurancePolicies.AsQueryable();

        if (tripId.HasValue)
            query = query.Where(p => p.TripId == tripId.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (!string.IsNullOrEmpty(policyNumber))
            query = query.Where(p => p.PolicyNumber != null && p.PolicyNumber.Contains(policyNumber));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<InsurancePolicy> AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default)
    {
        _dbContext.InsurancePolicies.Add(policy);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return policy;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(InsurancePolicy policy, CancellationToken cancellationToken = default)
    {
        _dbContext.InsurancePolicies.Update(policy);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var policy = await _dbContext.InsurancePolicies
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (policy is null)
            return false;

        _dbContext.InsurancePolicies.Remove(policy);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<InsurancePolicy?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InsurancePolicies
            .FirstOrDefaultAsync(p => p.TripId == tripId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InsurancePolicy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InsurancePolicies
            .FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber, cancellationToken);
    }
}
