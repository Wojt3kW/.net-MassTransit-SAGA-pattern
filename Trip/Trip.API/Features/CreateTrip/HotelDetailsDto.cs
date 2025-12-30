namespace Trip.API.Features.CreateTrip;

public record HotelDetailsDto(
    string HotelId,
    string HotelName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumberOfGuests);
