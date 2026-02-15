using MediatR;

namespace Trip.API.Features.RefundTrip;

/// <summary>Command to request a refund for a completed trip.</summary>
public record RefundTripCommand(Guid TripId, string Reason) : IRequest<bool>;
