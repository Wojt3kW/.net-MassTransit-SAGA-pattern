using Microsoft.EntityFrameworkCore;
using Trip.Application.Abstractions;
using Trip.Domain.Entities;
using Trip.Infrastructure.Persistence;

namespace Trip.Infrastructure.Repositories;

public class TripRepository : ITripRepository
{
    private readonly TripDbContext _dbContext;

    public TripRepository(TripDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TripBooking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TripBookings
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<TripBooking> AddAsync(TripBooking tripBooking, CancellationToken cancellationToken = default)
    {
        _dbContext.TripBookings.Add(tripBooking);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return tripBooking;
    }

    public async Task UpdateAsync(TripBooking tripBooking, CancellationToken cancellationToken = default)
    {
        _dbContext.TripBookings.Update(tripBooking);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TripBooking>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TripBookings
            .Where(t => t.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }
}
