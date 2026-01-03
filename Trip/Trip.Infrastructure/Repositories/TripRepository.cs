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

    public async Task<bool> UpdateOutboundFlightConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.TripBookings
            .Where(t => t.Id == tripId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.OutboundFlightConfirmation, confirmationCode), cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateReturnFlightConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.TripBookings
            .Where(t => t.Id == tripId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.ReturnFlightConfirmation, confirmationCode), cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateHotelConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.TripBookings
            .Where(t => t.Id == tripId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.HotelConfirmation, confirmationCode), cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateInsurancePolicyNumberAsync(Guid tripId, string policyNumber, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.TripBookings
            .Where(t => t.Id == tripId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.InsurancePolicyNumber, policyNumber), cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdatePaymentConfirmationAsync(Guid tripId, string confirmationCode, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.TripBookings
            .Where(t => t.Id == tripId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.PaymentConfirmation, confirmationCode), cancellationToken);
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateStatusAsync(Guid tripId, TripStatus status, DateTime? completedAt = null, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _dbContext.TripBookings
            .Where(t => t.Id == tripId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Status, status)
                .SetProperty(t => t.CompletedAt, completedAt), cancellationToken);
        return rowsAffected > 0;
    }
}
