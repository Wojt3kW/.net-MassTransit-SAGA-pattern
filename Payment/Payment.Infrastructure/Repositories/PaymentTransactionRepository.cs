using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;

namespace Payment.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for payment transaction data access operations.
/// </summary>
public class PaymentTransactionRepository : IPaymentTransactionRepository
{
    private readonly PaymentDbContext _dbContext;

    /// <summary>Initializes a new instance of the PaymentTransactionRepository.</summary>
    /// <param name="dbContext">The database context.</param>
    public PaymentTransactionRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<PaymentTransaction> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        PaymentStatus? status = null,
        Guid? transactionId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PaymentTransactions.AsQueryable();

        if (tripId.HasValue)
            query = query.Where(t => t.TripId == tripId.Value);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (transactionId.HasValue)
            query = query.Where(t => t.Id == transactionId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<PaymentTransaction> AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.PaymentTransactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
    {
        _dbContext.PaymentTransactions.Update(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (transaction is null)
            return false;

        _dbContext.PaymentTransactions.Remove(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<PaymentTransaction?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(p => p.TripId == tripId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaymentTransaction?> GetByAuthorisationCodeAsync(string authorisationCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentTransactions
            .FirstOrDefaultAsync(p => p.AuthorisationCode == authorisationCode, cancellationToken);
    }
}
