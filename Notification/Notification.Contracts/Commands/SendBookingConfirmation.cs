namespace Notification.Contracts.Commands;

public record SendBookingConfirmation(
    Guid TripId,
    Guid CustomerId,
    string CustomerEmail,
    string CustomerName,
    BookingDetails BookingDetails);
