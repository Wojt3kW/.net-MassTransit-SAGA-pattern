using HotelBooking.Domain.Entities;

namespace HotelBooking.Application.Abstractions;

/// <summary>
/// Repository interface for hotel reservation data access.
/// </summary>
public interface IHotelReservationRepository
{
    /// <summary>Gets a reservation by its unique identifier.</summary>
    Task<HotelReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a reservation by trip identifier.</summary>
    Task<HotelReservation?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>Gets all reservations with optional filtering and pagination.</summary>
    Task<(IReadOnlyList<HotelReservation> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        HotelReservationStatus? status = null,
        string? hotelId = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>Gets all expired reservations that need processing.</summary>
    Task<IReadOnlyList<HotelReservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new reservation.</summary>
    Task<HotelReservation> AddAsync(HotelReservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing reservation.</summary>
    Task UpdateAsync(HotelReservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Deletes a reservation by its identifier.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
