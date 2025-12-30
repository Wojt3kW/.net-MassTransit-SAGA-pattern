namespace Notification.Contracts.Commands;

public record BookingDetails(
    string OutboundFlightConfirmation,
    string ReturnFlightConfirmation,
    string HotelConfirmation,
    string? GroundTransportConfirmation,
    string? InsurancePolicyNumber,
    decimal TotalAmount);
