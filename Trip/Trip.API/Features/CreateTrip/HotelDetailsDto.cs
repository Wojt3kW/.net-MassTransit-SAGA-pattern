namespace Trip.API.Features.CreateTrip;

public record HotelDetailsDto(
    Guid HotelId,
    string HotelName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumberOfGuests);
