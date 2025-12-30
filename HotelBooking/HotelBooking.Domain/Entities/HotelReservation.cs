namespace HotelBooking.Domain.Entities;

/// <summary>
/// Represents a hotel accommodation reservation.
/// </summary>
public class HotelReservation
{
    /// <summary>Unique identifier of the hotel reservation.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Reference to the parent trip booking.</summary>
    public Guid TripId { get; set; }
    
    /// <summary>External identifier of the hotel in the booking system.</summary>
    public string HotelId { get; set; } = default!;
    
    /// <summary>Display name of the hotel.</summary>
    public string HotelName { get; set; } = default!;
    
    /// <summary>Check-in date and time.</summary>
    public DateTime CheckIn { get; set; }
    
    /// <summary>Check-out date and time.</summary>
    public DateTime CheckOut { get; set; }
    
    /// <summary>Number of guests staying at the hotel.</summary>
    public int NumberOfGuests { get; set; }
    
    /// <summary>Primary guest name for the reservation.</summary>
    public string GuestName { get; set; } = default!;
    
    /// <summary>Guest email for hotel communications.</summary>
    public string GuestEmail { get; set; } = default!;
    
    /// <summary>Hotel confirmation code for the reservation.</summary>
    public string? ConfirmationCode { get; set; }
    
    /// <summary>Room rate per night.</summary>
    public decimal PricePerNight { get; set; }
    
    /// <summary>Total price for the entire stay.</summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>Current status of the reservation.</summary>
    public HotelReservationStatus Status { get; set; }
    
    /// <summary>Reason for cancellation, if cancelled.</summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>Timestamp when the reservation was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when the reservation was confirmed.</summary>
    public DateTime? ConfirmedAt { get; set; }
    
    /// <summary>Timestamp when the reservation was cancelled.</summary>
    public DateTime? CancelledAt { get; set; }
    
    /// <summary>Timestamp when the provisional reservation expires.</summary>
    public DateTime? ExpiresAt { get; set; }
}
