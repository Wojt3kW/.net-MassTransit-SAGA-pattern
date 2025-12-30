namespace Insurance.Domain.Entities;

/// <summary>
/// Represents a travel insurance policy covering the trip.
/// </summary>
public class InsurancePolicy
{
    /// <summary>Unique identifier of the insurance policy.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Reference to the parent trip booking.</summary>
    public Guid TripId { get; set; }
    
    /// <summary>Customer who is the policyholder.</summary>
    public Guid CustomerId { get; set; }
    
    /// <summary>Full name of the insured customer.</summary>
    public string CustomerName { get; set; } = default!;
    
    /// <summary>Email address for policy communications.</summary>
    public string CustomerEmail { get; set; } = default!;
    
    /// <summary>Unique policy number issued by the insurer.</summary>
    public string? PolicyNumber { get; set; }
    
    /// <summary>Date when insurance coverage begins.</summary>
    public DateTime CoverageStartDate { get; set; }
    
    /// <summary>Date when insurance coverage ends.</summary>
    public DateTime CoverageEndDate { get; set; }
    
    /// <summary>Reference to the covered outbound flight reservation.</summary>
    public Guid OutboundFlightReservationId { get; set; }
    
    /// <summary>Reference to the covered return flight reservation.</summary>
    public Guid ReturnFlightReservationId { get; set; }
    
    /// <summary>Reference to the covered hotel reservation.</summary>
    public Guid HotelReservationId { get; set; }
    
    /// <summary>Total value of the trip being insured.</summary>
    public decimal TripTotalValue { get; set; }
    
    /// <summary>Insurance premium amount.</summary>
    public decimal Premium { get; set; }
    
    /// <summary>Current status of the insurance policy.</summary>
    public InsurancePolicyStatus Status { get; set; }
    
    /// <summary>Reason for policy cancellation, if cancelled.</summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>Timestamp when the policy was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when the policy was officially issued.</summary>
    public DateTime? IssuedAt { get; set; }
    
    /// <summary>Timestamp when the policy was cancelled.</summary>
    public DateTime? CancelledAt { get; set; }
}
