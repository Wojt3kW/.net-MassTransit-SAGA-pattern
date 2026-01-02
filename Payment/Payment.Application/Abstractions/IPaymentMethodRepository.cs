using Payment.Domain.Entities;

namespace Payment.Application.Abstractions;

/// <summary>
/// Repository interface for payment method data access operations.
/// </summary>
public interface IPaymentMethodRepository
{
    /// <summary>Gets a payment method by its unique identifier.</summary>
    /// <param name="id">The payment method ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment method if found; otherwise, null.</returns>
    Task<PaymentMethod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets the default payment method for a customer.</summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The default payment method if found; otherwise, null.</returns>
    Task<PaymentMethod?> GetDefaultByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>Gets all payment methods for a customer.</summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of payment methods for the customer.</returns>
    Task<IReadOnlyList<PaymentMethod>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new payment method.</summary>
    /// <param name="paymentMethod">The payment method to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);

    /// <summary>Deletes a payment method by its unique identifier.</summary>
    /// <param name="id">The payment method ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted; false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
