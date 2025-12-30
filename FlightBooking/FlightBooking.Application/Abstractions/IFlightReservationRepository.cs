using FlightBooking.Domain.Entities;

namespace FlightBooking.Application.Abstractions;

/// <summary>
/// Repository interface for flight reservation data access.
/// </summary>
public interface IFlightReservationRepository
{
    /// <summary>Gets a reservation by its unique identifier.</summary>
    Task<FlightReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets all reservations for a specific trip.</summary>
    Task<IReadOnlyList<FlightReservation>> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);

    /// <summary>Gets a reservation by trip identifier and flight type.</summary>
    Task<FlightReservation?> GetByTripIdAndTypeAsync(Guid tripId, FlightType type, CancellationToken cancellationToken = default);

    /// <summary>Gets all reservations with optional filtering and pagination.</summary>
    Task<(IReadOnlyList<FlightReservation> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        ReservationStatus? status = null,
        FlightType? type = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new reservation.</summary>
    Task<FlightReservation> AddAsync(FlightReservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing reservation.</summary>
    Task UpdateAsync(FlightReservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Deletes a reservation by its identifier.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
