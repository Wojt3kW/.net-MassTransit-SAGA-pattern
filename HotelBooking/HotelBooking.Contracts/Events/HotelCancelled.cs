namespace HotelBooking.Contracts.Events;

/// <summary>Published when a hotel reservation has been successfully cancelled during compensation.</summary>
public record HotelCancelled(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId,
    DateTime CancelledAt,
    string Reason);
