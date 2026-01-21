namespace TripBooking.Saga.StateMachines;

/// <summary>
/// Configuration settings for TripBookingStateMachine timeouts and retries.
/// </summary>
public record TripBookingStateMachineSettings
{
    /// <summary>Timeout for payment authorisation.</summary>
    public TimeSpan PaymentAuthorisationTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>Timeout for outbound flight reservation.</summary>
    public TimeSpan OutboundFlightReservationTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>Timeout for return flight reservation.</summary>
    public TimeSpan ReturnFlightReservationTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>Timeout for hotel reservation.</summary>
    public TimeSpan HotelReservationTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>Timeout for hotel confirmation.</summary>
    public TimeSpan HotelConfirmationTimeout { get; init; } = TimeSpan.FromMinutes(15);

    /// <summary>Timeout for ground transport reservation.</summary>
    public TimeSpan GroundTransportReservationTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>Timeout for insurance issuing.</summary>
    public TimeSpan InsuranceIssuingTimeout { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>Timeout for payment capture.</summary>
    public TimeSpan PaymentCaptureTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>Maximum retry attempts for payment capture.</summary>
    public int PaymentCaptureMaxRetries { get; init; } = 3;
}
