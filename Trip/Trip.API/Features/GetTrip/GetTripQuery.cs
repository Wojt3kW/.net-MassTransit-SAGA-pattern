using MediatR;
using Trip.Contracts.DTOs;

namespace Trip.API.Features.GetTrip;

public record GetTripQuery(Guid TripId) : IRequest<TripBookingStatusResponse?>;
