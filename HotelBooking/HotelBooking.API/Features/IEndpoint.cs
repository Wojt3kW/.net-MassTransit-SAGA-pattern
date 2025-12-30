namespace HotelBooking.API.Features;

/// <summary>
/// Interface for Minimal API endpoint registration.
/// </summary>
public interface IEndpoint
{
    /// <summary>Maps the endpoint to the application route builder.</summary>
    void MapEndpoint(IEndpointRouteBuilder app);
}
