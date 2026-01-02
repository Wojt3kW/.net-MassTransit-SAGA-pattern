using MediatR;

namespace TripBooking.Saga.API.Features.GetSagaState;

/// <summary>
/// Query to retrieve the current state of a saga by trip ID.
/// </summary>
public record GetSagaStateQuery(Guid TripId) : IRequest<SagaStateResponse?>;
