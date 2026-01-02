namespace Trip.Contracts.Events;

/// <summary>
/// Published when a new trip booking request is initiated, starting the SAGA orchestration.
/// Contains all data needed for the entire booking process.
/// </summary>
/// <param name="TripId">Unique identifier for the trip booking.</param>
/// <param name="CustomerId">Customer making the booking.</param>
/// <param name="CustomerName">Full name of the customer.</param>
/// <param name="CustomerEmail">Email address for notifications.</param>
/// <param name="Origin">Departure city or airport code.</param>
/// <param name="Destination">Arrival city or airport code.</param>
/// <param name="DepartureDate">Outbound flight departure date.</param>
/// <param name="ReturnDate">Return flight departure date.</param>
/// <param name="OutboundFlightNumber">Outbound flight number.</param>
/// <param name="OutboundCarrier">Outbound flight carrier.</param>
/// <param name="ReturnFlightNumber">Return flight number.</param>
/// <param name="ReturnCarrier">Return flight carrier.</param>
/// <param name="HotelId">Hotel identifier.</param>
/// <param name="HotelName">Hotel name.</param>
/// <param name="CheckIn">Hotel check-in date.</param>
/// <param name="CheckOut">Hotel check-out date.</param>
/// <param name="NumberOfGuests">Number of hotel guests.</param>
/// <param name="IncludeGroundTransport">Whether to include ground transport.</param>
/// <param name="GroundTransportType">Type of ground transport (if included).</param>
/// <param name="GroundTransportPickupLocation">Pickup location (if included).</param>
/// <param name="GroundTransportDropoffLocation">Dropoff location (if included).</param>
/// <param name="IncludeInsurance">Whether to include travel insurance.</param>
/// <param name="PaymentMethodId">Optional specific payment method. If null, uses default.</param>
/// <param name="TotalAmount">Total booking amount.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="CreatedAt">Timestamp when booking was created.</param>
public record TripBookingCreated(
    Guid TripId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string Origin,
    string Destination,
    DateTime DepartureDate,
    DateTime ReturnDate,
    string OutboundFlightNumber,
    string OutboundCarrier,
    string ReturnFlightNumber,
    string ReturnCarrier,
    Guid HotelId,
    string HotelName,
    DateTime CheckIn,
    DateTime CheckOut,
    int NumberOfGuests,
    bool IncludeGroundTransport,
    string? GroundTransportType,
    string? GroundTransportPickupLocation,
    string? GroundTransportDropoffLocation,
    bool IncludeInsurance,
    Guid? PaymentMethodId,
    decimal TotalAmount,
    string Currency,
    DateTime CreatedAt);
