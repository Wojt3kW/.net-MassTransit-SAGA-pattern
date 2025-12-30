namespace HotelBooking.Contracts.Events;

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
