using MediatR;

namespace Trip.API.Features.CancelTrip;

public record CancelTripCommand(Guid TripId, string Reason) : IRequest<bool>;
