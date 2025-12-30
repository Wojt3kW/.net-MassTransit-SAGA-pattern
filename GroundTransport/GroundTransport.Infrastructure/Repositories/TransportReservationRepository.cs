using GroundTransport.Application.Abstractions;
using GroundTransport.Domain.Entities;
using GroundTransport.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GroundTransport.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing transport reservation persistence.
/// </summary>
public class TransportReservationRepository : ITransportReservationRepository
{
    private readonly GroundTransportDbContext _dbContext;

    /// <summary>Initializes a new instance of the repository.</summary>
    public TransportReservationRepository(GroundTransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<TransportReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TransportReservations
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<TransportReservation> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        TransportReservationStatus? status = null,
        TransportType? type = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TransportReservations.AsQueryable();

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

    /// <inheritdoc />
    public async Task<TransportReservation> AddAsync(TransportReservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.TransportReservations.Add(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TransportReservation reservation, CancellationToken cancellationToken = default)
    {
        _dbContext.TransportReservations.Update(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var reservation = await _dbContext.TransportReservations
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (reservation is null)
            return false;

        _dbContext.TransportReservations.Remove(reservation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<TransportReservation?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TransportReservations
            .FirstOrDefaultAsync(t => t.TripId == tripId, cancellationToken);
    }
}
