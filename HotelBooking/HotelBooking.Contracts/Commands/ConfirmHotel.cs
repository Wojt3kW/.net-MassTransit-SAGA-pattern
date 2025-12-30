namespace HotelBooking.Contracts.Commands;

public record ConfirmHotel(
    Guid CorrelationId,
    Guid TripId,
    Guid HotelReservationId);
