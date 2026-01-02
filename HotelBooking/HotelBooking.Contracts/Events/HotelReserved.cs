namespace HotelBooking.Contracts.Events;

/// <summary>Published when a hotel room has been successfully reserved (pending confirmation).</summary>
public record HotelReserved(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId,
    string HotelName,
    string ConfirmationCode,
    DateTime CheckIn,
    DateTime CheckOut,
    decimal PricePerNight,
    decimal TotalPrice);
