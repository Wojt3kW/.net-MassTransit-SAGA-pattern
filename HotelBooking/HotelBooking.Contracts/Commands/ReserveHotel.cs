namespace HotelBooking.Contracts.Commands;

public record ReserveHotel(
    Guid CorrelationId,
    Guid TripId,
    string HotelId,
    string HotelName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumberOfGuests,
    string GuestName,
    string GuestEmail);
