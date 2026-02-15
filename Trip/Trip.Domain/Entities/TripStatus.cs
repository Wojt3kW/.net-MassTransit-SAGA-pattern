namespace Trip.Domain.Entities;

/// <summary>Represents the current status of a trip booking.</summary>
public enum TripStatus
{
    /// <summary>Trip booking created but not yet processing.</summary>
    Pending,

    /// <summary>Trip booking is being processed by the SAGA.</summary>
    Processing,

    /// <summary>Trip booking completed successfully.</summary>
    Completed,

    /// <summary>Trip booking failed during processing.</summary>
    Failed,

    /// <summary>Trip booking was cancelled by the user.</summary>
    Cancelled,

    /// <summary>Trip booking was refunded after completion.</summary>
    Refunded
}
