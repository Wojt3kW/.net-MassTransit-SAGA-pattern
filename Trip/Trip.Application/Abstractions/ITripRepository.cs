using Trip.Domain.Entities;

namespace Trip.Application.Abstractions;

public interface ITripRepository
{
    Task<TripBooking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TripBooking> AddAsync(TripBooking tripBooking, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<TripBooking> Items, int TotalCount)> GetAllAsync(
        TripStatus? status = null,
        Guid? customerId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    // Atomic field updates to prevent lost updates in concurrent scenarios
    Task<bool> UpdateOutboundFlightConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default);
    Task<bool> UpdateReturnFlightConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default);
    Task<bool> UpdateHotelConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default);
    Task<bool> UpdateInsurancePolicyNumberAsync(Guid tripId, string policyNumber, CancellationToken cancellationToken = default);
    Task<bool> UpdatePaymentConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid tripId, TripStatus status, DateTime? completedAt = null, CancellationToken cancellationToken = default);
}
