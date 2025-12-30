using HotelBooking.Application.Abstractions;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for hotel reservation data access.
/// </summary>
public class HotelReservationRepository : IHotelReservationRepository
{
    private readonly HotelBookingDbContext _dbContext;

    public HotelReservationRepository(HotelBookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HotelReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HotelReservations
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<HotelReservation?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.HotelReservations
            .FirstOrDefaultAsync(h => h.TripId == tripId, cancellationToken);
    }

    public async Task<(IReadOnlyList<HotelReservation> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        HotelReservationStatus? status = null,
        string? hotelId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.HotelReservations.AsQueryable();

        if (tripId.HasValue)
            query = query.Where(r => r.TripId == tripId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (!string.IsNullOrEmpty(hotelId))
            query = query.Where(r => r.HotelId == hotelId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<HotelReservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.HotelReservations
            .Where(h => h.Status == HotelReservationStatus.Reserved && h.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<HotelReservation> AddAsync(HotelReservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.HotelReservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task UpdateAsync(HotelReservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.HotelReservations.Update(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.HotelReservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reservation is null)
            return false;

        _dbContext.HotelReservations.Remove(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
