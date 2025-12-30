namespace GroundTransport.Domain.Entities;

/// <summary>
/// Represents a ground transport reservation (airport transfer or car rental).
/// </summary>
public class TransportReservation
{
    /// <summary>Unique identifier of the transport reservation.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Reference to the parent trip booking.</summary>
    public Guid TripId { get; set; }
    
    /// <summary>Type of transport: AirportTransfer or CarRental.</summary>
    public TransportType Type { get; set; }
    
    /// <summary>Address or location for pickup.</summary>
    public string PickupLocation { get; set; } = default!;
    
    /// <summary>Address or location for drop-off.</summary>
    public string DropoffLocation { get; set; } = default!;
    
    /// <summary>Scheduled pickup date and time.</summary>
    public DateTime PickupDate { get; set; }
    
    /// <summary>Number of passengers requiring transport.</summary>
    public int PassengerCount { get; set; }
    
    /// <summary>Transport provider confirmation code.</summary>
    public string? ConfirmationCode { get; set; }
    
    /// <summary>Total price for the transport service.</summary>
    public decimal Price { get; set; }
    
    /// <summary>Current status of the reservation.</summary>
    public TransportReservationStatus Status { get; set; }
    
    /// <summary>Reason for cancellation, if cancelled.</summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>Timestamp when the reservation was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when the reservation was confirmed.</summary>
    public DateTime? ConfirmedAt { get; set; }
    
    /// <summary>Timestamp when the reservation was cancelled.</summary>
    public DateTime? CancelledAt { get; set; }
}
