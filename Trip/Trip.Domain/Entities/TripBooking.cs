namespace Trip.Domain.Entities;

/// <summary>
/// Represents a complete travel booking containing all trip components.
/// </summary>
public class TripBooking
{
    /// <summary>Unique identifier of the trip booking.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Customer who made the booking.</summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>Email address for booking notifications.</summary>
    public string CustomerEmail { get; set; } = default!;
    
    /// <summary>Current status of the trip booking process.</summary>
    public TripStatus Status { get; set; }
    
    /// <summary>Confirmation code for outbound flight reservation.</summary>
    public string? OutboundFlightConfirmation { get; set; }
    
    /// <summary>Confirmation code for return flight reservation.</summary>
    public string? ReturnFlightConfirmation { get; set; }
    
    /// <summary>Confirmation code for hotel reservation.</summary>
    public string? HotelConfirmation { get; set; }
    
    /// <summary>Confirmation code for ground transport reservation.</summary>
    public string? GroundTransportConfirmation { get; set; }
    
    /// <summary>Policy number for travel insurance.</summary>
    public string? InsurancePolicyNumber { get; set; }
    
    /// <summary>Confirmation code for payment transaction.</summary>
    public string? PaymentConfirmation { get; set; }
    
    /// <summary>Total amount charged for the entire trip.</summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>Reason for booking failure, if applicable.</summary>
    public string? FailureReason { get; set; }
    
    /// <summary>Timestamp when the booking was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when the booking was successfully completed.</summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>Timestamp when the booking was cancelled.</summary>
    public DateTime? CancelledAt { get; set; }
}
