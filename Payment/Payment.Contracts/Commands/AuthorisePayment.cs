namespace Payment.Contracts.Commands;

/// <summary>
/// Command to authorise (block) payment funds for a trip booking.
/// </summary>
/// <param name="CorrelationId">Saga correlation identifier.</param>
/// <param name="TripId">Trip booking identifier.</param>
/// <param name="CustomerId">Customer identifier.</param>
/// <param name="PaymentMethodId">Optional specific payment method. If null, uses customer's default.</param>
/// <param name="Amount">Amount to authorise.</param>
/// <param name="Currency">Currency code (e.g., "GBP", "USD").</param>
public record AuthorisePayment(
    Guid CorrelationId,
    Guid TripId,
    Guid CustomerId,
    Guid? PaymentMethodId,
    decimal Amount,
    string Currency);
