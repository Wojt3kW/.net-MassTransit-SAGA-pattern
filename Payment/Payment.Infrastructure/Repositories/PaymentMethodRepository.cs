using Microsoft.EntityFrameworkCore;
using Payment.Application.Abstractions;
using Payment.Domain.Entities;
using Payment.Infrastructure.Persistence;

namespace Payment.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for payment method data access operations.
/// </summary>
public class PaymentMethodRepository : IPaymentMethodRepository
{
    private readonly PaymentDbContext _dbContext;

    /// <summary>Initializes a new instance of the PaymentMethodRepository.</summary>
    /// <param name="dbContext">The database context.</param>
    public PaymentMethodRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaymentMethod?> GetDefaultByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentMethods
            .FirstOrDefaultAsync(p => p.CustomerId == customerId && p.IsDefault, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PaymentMethod>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PaymentMethods
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.IsDefault)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        _dbContext.PaymentMethods.Add(paymentMethod);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _dbContext.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (paymentMethod is null)
            return false;

        _dbContext.PaymentMethods.Remove(paymentMethod);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
