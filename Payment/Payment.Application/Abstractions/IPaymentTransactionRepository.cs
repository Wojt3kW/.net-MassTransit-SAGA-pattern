using Payment.Domain.Entities;

namespace Payment.Application.Abstractions;

/// <summary>
/// Repository interface for payment transaction data access operations.
/// </summary>
public interface IPaymentTransactionRepository
{
    /// <summary>Gets a payment transaction by its unique identifier.</summary>
    /// <param name="id">The payment transaction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment transaction if found; otherwise, null.</returns>
    Task<PaymentTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets all payment transactions with optional filtering and pagination.</summary>
    /// <param name="tripId">Optional filter by trip ID.</param>
    /// <param name="status">Optional filter by payment status.</param>
    /// <param name="transactionId">Optional filter by transaction ID.</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the list of transactions and total count.</returns>
    Task<(IReadOnlyList<PaymentTransaction> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        PaymentStatus? status = null,
        Guid? transactionId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new payment transaction.</summary>
    /// <param name="transaction">The payment transaction to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The added payment transaction.</returns>
    Task<PaymentTransaction> AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing payment transaction.</summary>
    /// <param name="transaction">The payment transaction to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>Deletes a payment transaction by its unique identifier.</summary>
    /// <param name="id">The payment transaction ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the transaction was deleted; false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a payment transaction by trip ID.</summary>
    /// <param name="tripId">The trip ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment transaction if found; otherwise, null.</returns>
    Task<PaymentTransaction?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>Gets a payment transaction by authorisation code.</summary>
    /// <param name="authorisationCode">The authorisation code from the payment gateway.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment transaction if found; otherwise, null.</returns>
    Task<PaymentTransaction?> GetByAuthorisationCodeAsync(string authorisationCode, CancellationToken cancellationToken = default);
}
