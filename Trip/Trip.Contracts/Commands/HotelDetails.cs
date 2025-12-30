namespace Trip.Contracts.Commands;

public record HotelDetails(
    string HotelId,
    string HotelName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumberOfGuests);
