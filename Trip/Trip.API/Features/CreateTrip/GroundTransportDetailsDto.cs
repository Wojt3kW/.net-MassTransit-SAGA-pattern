namespace Trip.API.Features.CreateTrip;

public record GroundTransportDetailsDto(
    string Type,
    string PickupLocation,
    string DropoffLocation,
    DateTime PickupDate);
