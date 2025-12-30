using GroundTransport.Domain.Entities;

namespace GroundTransport.Application.Abstractions;

/// <summary>
/// Repository interface for managing transport reservation persistence.
/// </summary>
public interface ITransportReservationRepository
{
    /// <summary>Gets a transport reservation by its unique identifier.</summary>
    Task<TransportReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets all transport reservations with optional filtering and pagination.</summary>
    Task<(IReadOnlyList<TransportReservation> Items, int TotalCount)> GetAllAsync(
        Guid? tripId = null,
        TransportReservationStatus? status = null,
        TransportType? type = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>Adds a new transport reservation to the repository.</summary>
    Task<TransportReservation> AddAsync(TransportReservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing transport reservation.</summary>
    Task UpdateAsync(TransportReservation reservation, CancellationToken cancellationToken = default);

    /// <summary>Deletes a transport reservation by its unique identifier.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Gets a transport reservation by trip identifier.</summary>
    Task<TransportReservation?> GetByTripIdAsync(Guid tripId, CancellationToken cancellationToken = default);
}
