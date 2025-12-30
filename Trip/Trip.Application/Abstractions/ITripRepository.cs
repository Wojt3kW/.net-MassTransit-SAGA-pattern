using Trip.Domain.Entities;

namespace Trip.Application.Abstractions;

public interface ITripRepository
{
    Task<TripBooking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TripBooking> AddAsync(TripBooking tripBooking, CancellationToken cancellationToken = default);
    Task UpdateAsync(TripBooking tripBooking, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TripBooking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
