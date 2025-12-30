using FlightBooking.Application.Abstractions;
using FlightBooking.Domain.Entities;
using FlightBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for flight reservation data access.
/// </summary>
public class FlightReservationRepository : IFlightReservationRepository
{
    private readonly FlightBookingDbContext _dbContext;

    public FlightReservationRepository(FlightBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FlightReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FlightReservations
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FlightReservation>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FlightReservations
            .Where(f => f.TripId == tripId)
            .ToListAsync(cancellationToken);
    }

    public async Task<FlightReservation?> GetByTripIdAndTypeAsync(Guid tripId, FlightType type, CancellationToken cancellationToken = default)
    {
        return await _dbContext.FlightReservations
            .FirstOrDefaultAsync(f => f.TripId == tripId && f.Type == type, cancellationToken);
    }

    public async Task<(IReadOnlyList<FlightReservation> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        ReservationStatus? status = null,
        FlightType? type = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.FlightReservations.AsQueryable();

        if (tripId.HasValue)
            query = query.Where(r => r.TripId == tripId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (type.HasValue)
            query = query.Where(r => r.Type == type.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<FlightReservation> AddAsync(FlightReservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.FlightReservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task UpdateAsync(FlightReservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.FlightReservations.Update(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.FlightReservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reservation is null)
            return false;

        _dbContext.FlightReservations.Remove(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
