using MediatR;
using Trip.Contracts.DTOs;

namespace Trip.API.Features.CreateTrip;

public record CreateTripCommand(
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    TripDetailsDto Details) : IRequest<TripBookingResponse>;
