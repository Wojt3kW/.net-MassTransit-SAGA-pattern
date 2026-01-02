namespace Payment.Domain.Entities;

/// <summary>
/// Represents a stored payment method for a customer.
/// </summary>
/// <remarks>
/// ⚠️ FOR EDUCATIONAL PURPOSES ONLY!
/// In production, card data should NEVER be stored directly.
/// </remarks>
public class PaymentMethod
{
    /// <summary>Unique identifier of the payment method.</summary>
    public Guid Id { get; set; }

    /// <summary>Customer who owns this payment method.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Display name for the payment method (e.g., "Personal Visa").</summary>
    public string Name { get; set; } = default!;

    /// <summary>Last four digits of the card (for display).</summary>
    public string CardLastFourDigits { get; set; } = default!;

    /// <summary>Full card number (encrypted in production).</summary>
    /// <remarks>⚠️ FOR EDUCATIONAL PURPOSES ONLY! Never store raw card numbers.</remarks>
    public string CardNumber { get; set; } = default!;

    /// <summary>Name on the payment card.</summary>
    public string CardHolderName { get; set; } = default!;

    /// <summary>Card expiry date in MM/YY format.</summary>
    public string ExpiryDate { get; set; } = default!;

    /// <summary>Card security code.</summary>
    /// <remarks>⚠️ FOR EDUCATIONAL PURPOSES ONLY! CVV must NEVER be stored.</remarks>
    public string Cvv { get; set; } = default!;

    /// <summary>Whether this is the customer's default payment method.</summary>
    public bool IsDefault { get; set; }

    /// <summary>Timestamp when the payment method was added.</summary>
    public DateTime CreatedAt { get; set; }
}
