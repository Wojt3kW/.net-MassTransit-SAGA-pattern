namespace Payment.Domain.Entities;

/// <summary>
/// Represents a payment transaction for the trip booking.
/// </summary>
public class PaymentTransaction
{
    /// <summary>Unique identifier of the payment transaction.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Reference to the parent trip booking.</summary>
    public Guid TripId { get; set; }
    
    /// <summary>Customer making the payment.</summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>Last four digits of the payment card (for display).</summary>
    public string CardLastFourDigits { get; set; } = default!;
    
    /// <summary>Name of the cardholder.</summary>
    public string CardHolderName { get; set; } = default!;
    
    /// <summary>Payment amount.</summary>
    public decimal Amount { get; set; }
    
    /// <summary>Currency code (e.g., "GBP", "USD").</summary>
    public string Currency { get; set; } = default!;
    
    /// <summary>Authorisation code from the payment gateway.</summary>
    public string? AuthorisationCode { get; set; }
    
    /// <summary>Current status of the payment transaction.</summary>
    public PaymentStatus Status { get; set; }
    
    /// <summary>Reason for payment failure, if failed.</summary>
    public string? FailureReason { get; set; }
    
    /// <summary>Timestamp when the transaction was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when payment was authorised (funds blocked).</summary>
    public DateTime? AuthorisedAt { get; set; }
    
    /// <summary>Timestamp when payment was captured (funds charged).</summary>
    public DateTime? CapturedAt { get; set; }
    
    /// <summary>Timestamp when authorisation was released (funds unblocked).</summary>
    public DateTime? ReleasedAt { get; set; }
    
    /// <summary>Timestamp when payment was refunded.</summary>
    public DateTime? RefundedAt { get; set; }
}
