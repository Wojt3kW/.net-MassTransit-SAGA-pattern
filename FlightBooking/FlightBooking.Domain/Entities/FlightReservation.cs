namespace FlightBooking.Domain.Entities;

/// <summary>
/// Represents a flight reservation for outbound or return journey.
/// </summary>
public class FlightReservation
{
    /// <summary>Unique identifier of the flight reservation.</summary>
    public Guid Id { get; set; }
    
    /// <summary>Reference to the parent trip booking.</summary>
    public Guid TripId { get; set; }
    
    /// <summary>Type of flight: Outbound or Return.</summary>
    public FlightType Type { get; set; }
    
    /// <summary>Flight number (e.g., "BA123").</summary>
    public string FlightNumber { get; set; } = default!;
    
    /// <summary>Airline carrier name.</summary>
    public string Carrier { get; set; } = default!;
    
    /// <summary>Departure airport code (e.g., "LHR").</summary>
    public string Origin { get; set; } = default!;
    
    /// <summary>Arrival airport code (e.g., "JFK").</summary>
    public string Destination { get; set; } = default!;
    
    /// <summary>Scheduled departure date and time.</summary>
    public DateTime DepartureDate { get; set; }
    
    /// <summary>Airline confirmation code for the reservation.</summary>
    public string? ConfirmationCode { get; set; }
    
    /// <summary>Total price for all passengers.</summary>
    public decimal Price { get; set; }
    
    /// <summary>Number of passengers on this flight.</summary>
    public int PassengerCount { get; set; }
    
    /// <summary>Current status of the reservation.</summary>
    public ReservationStatus Status { get; set; }
    
    /// <summary>Reason for cancellation, if cancelled.</summary>
    public string? CancellationReason { get; set; }
    
    /// <summary>Timestamp when the reservation was created.</summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>Timestamp when the reservation was confirmed.</summary>
    public DateTime? ConfirmedAt { get; set; }
    
    /// <summary>Timestamp when the reservation was cancelled.</summary>
    public DateTime? CancelledAt { get; set; }
}
