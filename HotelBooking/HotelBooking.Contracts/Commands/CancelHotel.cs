namespace HotelBooking.Contracts.Commands;

public record CancelHotel(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId,
    string Reason);
