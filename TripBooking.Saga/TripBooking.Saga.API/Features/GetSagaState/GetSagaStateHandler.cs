using MediatR;
using Microsoft.EntityFrameworkCore;
using TripBooking.Saga.Persistence;

namespace TripBooking.Saga.API.Features.GetSagaState;

/// <summary>
/// Handler for retrieving saga state by trip ID.
/// </summary>
public class GetSagaStateHandler(TripBookingSagaDbContext dbContext) 
    : IRequestHandler<GetSagaStateQuery, SagaStateResponse?>
{
    public async Task<SagaStateResponse?> Handle(GetSagaStateQuery request, CancellationToken cancellationToken)
    {
        var saga = await dbContext.TripBookingSagaStates
            .FirstOrDefaultAsync(s => s.TripId == request.TripId, cancellationToken);

        if (saga is null)
            return null;

        return new SagaStateResponse(
            saga.CorrelationId,
            saga.TripId,
            saga.CurrentState,
            saga.CustomerId,
            saga.CustomerEmail,
            saga.Origin,
            saga.Destination,
            saga.DepartureDate,
            saga.ReturnDate,
            saga.TotalAmount,
            saga.PaymentTransactionId,
            saga.OutboundFlightId,
            saga.ReturnFlightId,
            saga.HotelReservationId,
            saga.GroundTransportId,
            saga.InsurancePolicyId,
            saga.IsPaymentAuthorised,
            saga.IsOutboundFlightReserved,
            saga.IsReturnFlightReserved,
            saga.IsHotelReserved,
            saga.IsHotelConfirmed,
            saga.IsGroundTransportReserved,
            saga.IsInsuranceIssued,
            saga.IsPaymentCaptured,
            saga.CreatedAt,
            saga.CompletedAt,
            saga.FailureReason
        );
    }
}
