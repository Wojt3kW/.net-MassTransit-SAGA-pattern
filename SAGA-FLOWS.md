# SAGA Flows - Travel Booking Platform

This document describes all SAGA flows, scenarios, and compensation strategies implemented in the Travel Booking Platform.

## Table of Contents

1. [Happy Path](#happy-path)
2. [Failure Scenarios](#failure-scenarios)
3. [Compensation Flows](#compensation-flows)
4. [Timeout Scenarios](#timeout-scenarios)
5. [User Cancellation](#user-cancellation)
6. [State Machine Diagram](#state-machine-diagram)

---

## Happy Path

The successful booking flow when all services respond positively.

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│                              HAPPY PATH FLOW                                         │
└──────────────────────────────────────────────────────────────────────────────────────┘

User Request (POST /api/trips)
    │
    ▼
┌───────────────────────┐
│ 1. TripBookingCreated │ ──────► Trip.API creates TripBooking record
└───────────────────────┘
    │
    ▼
┌─────────────────────┐
│ 2. AuthorisePayment │ ──────► Payment.API blocks funds on card
└─────────────────────┘
    │ PaymentAuthorised
    ▼
┌─────────────────────┐
│ 3. ReserveOutbound  │ ──────► FlightBooking.API reserves outbound flight
└─────────────────────┘
    │ OutboundFlightReserved
    ▼
┌─────────────────────┐
│ 4. ReserveReturn    │ ──────► FlightBooking.API reserves return flight
└─────────────────────┘
    │ ReturnFlightReserved
    ▼
┌─────────────────────┐
│ 5. ReserveHotel     │ ──────► HotelBooking.API creates reservation
└─────────────────────┘
    │ HotelReserved
    ▼
┌─────────────────────┐
│ 6. ConfirmHotel     │ ──────► HotelBooking.API confirms reservation
└─────────────────────┘
    │ HotelConfirmed
    ▼
┌─────────────────────┐
│ 7. GroundTransport? │ ──────► Optional: GroundTransport.API reserves transfer
└─────────────────────┘
    │ GroundTransportReserved (or skipped)
    ▼
┌─────────────────────┐
│ 8. IssueInsurance?  │ ──────► Optional: Insurance.API issues policy
└─────────────────────┘
    │ InsuranceIssued (or skipped)
    ▼
┌─────────────────────┐
│ 9. CapturePayment   │ ──────► Payment.API captures authorised funds
└─────────────────────┘
    │ PaymentCaptured
    ▼
┌─────────────────────┐
│ 10. COMPLETED ✓     │ ──────► TripBookingCompleted event published
└─────────────────────┘
```

### Happy Path Events Sequence

| Step | Command Sent | Event Received | Next State |
|------|--------------|----------------|------------|
| 1 | - | TripBookingCreated | AwaitingPaymentAuthorisation |
| 2 | AuthorisePayment | PaymentAuthorised | AwaitingOutboundFlight |
| 3 | ReserveOutboundFlight | OutboundFlightReserved | AwaitingReturnFlight |
| 4 | ReserveReturnFlight | ReturnFlightReserved | AwaitingHotel |
| 5 | ReserveHotel | HotelReserved | AwaitingHotelConfirmation |
| 6 | ConfirmHotel | HotelConfirmed | AwaitingGroundTransport |
| 7 | ReserveGroundTransport | GroundTransportReserved | AwaitingInsurance |
| 8 | IssueInsurance | InsuranceIssued | AwaitingPaymentCapture |
| 9 | CapturePayment | PaymentCaptured | Completed |

---

## Failure Scenarios

### Scenario 1: Payment Authorisation Failed

**When:** Customer's card is declined or payment method not found.

```
TripBookingCreated
    │
    ▼
AuthorisePayment ──► PaymentAuthorisationFailed
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │  No compensation needed (nothing to rollback)
└─────────────────────┘
```

**Compensation:** None required - no resources were reserved yet.

---

### Scenario 2: Outbound Flight Reservation Failed

**When:** Flight is fully booked, airline API error, or invalid flight details.

```
PaymentAuthorised
    │
    ▼
ReserveOutboundFlight ──► OutboundFlightReservationFailed
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Compensation Chain:**
1. ❌ Outbound flight failed
2. 🔄 Release payment authorisation

---

### Scenario 3: Return Flight Reservation Failed

**When:** Return flight unavailable after outbound was booked.

```
OutboundFlightReserved
    │
    ▼
ReserveReturnFlight ──► ReturnFlightReservationFailed
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Compensation Chain:**
1. ❌ Return flight failed
2. 🔄 Cancel outbound flight
3. 🔄 Release payment authorisation

---

### Scenario 4: Hotel Reservation Failed

**When:** Hotel has no availability or booking system error.

```
ReturnFlightReserved
    │
    ▼
ReserveHotel ──► HotelReservationFailed
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
CancelFlight (return) ──► FlightCancelled
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Compensation Chain:**
1. ❌ Hotel reservation failed
2. 🔄 Cancel return flight
3. 🔄 Cancel outbound flight
4. 🔄 Release payment authorisation

---

### Scenario 5: Hotel Confirmation Expired

**When:** Hotel reservation was not confirmed within 15-minute window.

```
HotelReserved
    │
    ▼
ConfirmHotel ──► HotelConfirmationExpired (timeout)
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
CancelHotel ──► HotelCancelled
    │
    ▼
CancelFlight (return) ──► FlightCancelled
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Compensation Chain:**
1. ❌ Hotel confirmation expired
2. 🔄 Cancel hotel reservation
3. 🔄 Cancel return flight
4. 🔄 Cancel outbound flight
5. 🔄 Release payment authorisation

---

### Scenario 6: Ground Transport Failed (Optional Service)

**When:** No vehicles available or service error.

```
HotelConfirmed
    │
    ▼
ReserveGroundTransport ──► GroundTransportReservationFailed
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
CancelHotel ──► HotelCancelled
    │
    ▼
CancelFlight (return) ──► FlightCancelled
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Note:** If `IncludeGroundTransport = false`, this step is skipped entirely.

---

### Scenario 7: Insurance Issuance Failed (Optional Service)

**When:** Insurance provider rejects the policy or service error.

```
GroundTransportReserved (or HotelConfirmed if no transport)
    │
    ▼
IssueInsurance ──► InsuranceIssueFailed
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
CancelGroundTransport ──► GroundTransportCancelled (if applicable)
    │
    ▼
CancelHotel ──► HotelCancelled
    │
    ▼
CancelFlight (return) ──► FlightCancelled
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Note:** If `IncludeInsurance = false`, this step is skipped entirely.

---

### Scenario 8: Payment Capture Failed

**When:** Card expired, insufficient funds, or payment gateway error.

```
InsuranceIssued (or previous step if no insurance)
    │
    ▼
CapturePayment ──► PaymentCaptureFailed
    │
    ▼
┌─────────────────────┐
│     RETRY (3x)      │  Retry up to 3 times with exponential backoff
└─────────────────────┘
    │ (if all retries fail)
    ▼
┌─────────────────────┐
│   COMPENSATING      │
└─────────────────────┘
    │
    ▼
CancelInsurance ──► InsuranceCancelled (if applicable)
    │
    ▼
CancelGroundTransport ──► GroundTransportCancelled (if applicable)
    │
    ▼
CancelHotel ──► HotelCancelled
    │
    ▼
CancelFlight (return) ──► FlightCancelled
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled
    │
    ▼
ReleasePayment ──► PaymentReleased
    │
    ▼
┌─────────────────────┐
│      FAILED ✗       │
└─────────────────────┘
```

**Compensation Chain (full):**
1. ❌ Payment capture failed after 3 retries
2. 🔄 Cancel insurance policy
3. 🔄 Cancel ground transport
4. 🔄 Cancel hotel reservation
5. 🔄 Cancel return flight
6. 🔄 Cancel outbound flight
7. 🔄 Release payment authorisation

---

## Compensation Flows

### Compensation Order (Reverse of Booking)

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                         COMPENSATION ORDER                                          │
└─────────────────────────────────────────────────────────────────────────────────────┘

When failure occurs, compensate in REVERSE order:

Payment Capture Failed:
    └── Cancel Insurance (if issued)
        └── Cancel Ground Transport (if reserved)
            └── Cancel Hotel
                └── Cancel Return Flight
                    └── Cancel Outbound Flight
                        └── Release Payment
```

### Compensation Matrix

| Failed At | Cancel Insurance | Cancel Transport | Cancel Hotel | Cancel Return | Cancel Outbound | Release Payment |
|-----------|:----------------:|:----------------:|:------------:|:-------------:|:---------------:|:---------------:|
| Payment Auth | - | - | - | - | - | - |
| Outbound Flight | - | - | - | - | - | ✓ |
| Return Flight | - | - | - | - | ✓ | ✓ |
| Hotel Reserve | - | - | - | ✓ | ✓ | ✓ |
| Hotel Confirm | - | - | ✓ | ✓ | ✓ | ✓ |
| Ground Transport | - | - | ✓ | ✓ | ✓ | ✓ |
| Insurance | - | ✓* | ✓ | ✓ | ✓ | ✓ |
| Payment Capture | ✓* | ✓* | ✓ | ✓ | ✓ | ✓ |

*Only if the optional service was included in the booking.

---

## Timeout Scenarios

### Timeout Configuration

| Step | Timeout | Action on Timeout |
|------|---------|-------------------|
| Payment Authorisation | 30 seconds | Fail immediately, no compensation |
| Outbound Flight | 60 seconds | Release payment |
| Return Flight | 60 seconds | Cancel outbound → Release payment |
| Hotel Reservation | 60 seconds | Cancel flights → Release payment |
| Hotel Confirmation | 15 minutes | Cancel all → Release payment |
| Ground Transport | 60 seconds | Cancel hotel, flights → Release payment |
| Insurance | 60 seconds | Full compensation cascade |
| Payment Capture | 30 seconds | Retry 3x, then full compensation cascade |

### Timeout Flow Example

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                      HOTEL CONFIRMATION TIMEOUT                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘

HotelReserved
    │
    ├── Start 15-minute timer (Schedule)
    │
    ▼
Waiting for ConfirmHotel...
    │
    │   ⏱️ 15 minutes pass without confirmation
    │
    ▼
HotelConfirmationExpired (Scheduled event fires)
    │
    ▼
Begin Compensation...
```

### Late Event Handling (Active Cancellation)

When a timeout occurs, the SAGA transitions to `Failed` state. However, the slow service may still complete its work and publish a success event. Instead of ignoring these "orphaned" resources, the SAGA actively cancels them.

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                      ACTIVE CANCELLATION OF LATE EVENTS                             │
└─────────────────────────────────────────────────────────────────────────────────────┘

Timeline:
    0s   ReserveOutboundFlight sent, 60s timeout scheduled
   60s   ⏱️ TIMEOUT! → ReleasingPayment → Failed
   65s   Late OutboundFlightReserved arrives
         │
         ▼
    ┌─────────────────────────────────────────┐
    │ During(Failed)                          │
    │   When(OutboundFlightReserved)          │
    │     → Publish(CancelFlight)             │  ← Active cancellation!
    │     → FlightReservation.Status=Cancelled│
    └─────────────────────────────────────────┘
```

**Events actively cancelled in Failed state:**
- `OutboundFlightReserved` → `CancelFlight`
- `ReturnFlightReserved` → `CancelFlight`
- `HotelReserved` → `CancelHotel`
- `GroundTransportReserved` → `CancelGroundTransport`
- `InsuranceIssued` → `CancelInsurance`

**Events ignored in Failed state (no action needed):**
- `FlightReservationFailed`, `FlightCancelled`
- `HotelConfirmed`, `HotelReservationFailed`, `HotelConfirmationExpired`, `HotelCancelled`
- `GroundTransportReservationFailed`, `GroundTransportCancelled`
- `InsuranceIssueFailed`, `InsuranceCancelled`
- `PaymentCaptured`, `PaymentCaptureFailed`, `PaymentReleased`

---

## User Cancellation

### Cancel Before Completion

User can request cancellation at any point before the booking is completed.

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                         USER CANCELLATION FLOW                                      │
└─────────────────────────────────────────────────────────────────────────────────────┘

User Request (POST /api/trips/{id}/cancel)
    │
    ▼
┌─────────────────────┐
│ CancelTripBooking   │ ──────► Check current SAGA state
└─────────────────────┘
    │
    ▼
┌─────────────────────┐
│   COMPENSATING      │ ──────► Compensate all completed steps
└─────────────────────┘
    │
    ▼
(Compensation based on current state - see matrix above)
    │
    ▼
┌─────────────────────┐
│    CANCELLED ✓      │
└─────────────────────┘
```

### Cancel After Completion (Refund)

If booking is already completed, a refund process is triggered.

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                         REFUND FLOW (Post-Completion)                               │
└─────────────────────────────────────────────────────────────────────────────────────┘

User Request (POST /api/trips/{id}/cancel)  [Status: Completed]
    │
    ▼
┌─────────────────────┐
│ CancelTripBooking   │
└─────────────────────┘
    │
    ▼
CancelInsurance ──► InsuranceCancelled (if applicable)
    │
    ▼
CancelGroundTransport ──► GroundTransportCancelled (if applicable)
    │
    ▼
CancelHotel ──► HotelCancelled (may incur cancellation fee)
    │
    ▼
CancelFlight (return) ──► FlightCancelled (may incur fee)
    │
    ▼
CancelFlight (outbound) ──► FlightCancelled (may incur fee)
    │
    ▼
RefundPayment ──► PaymentRefunded (minus any fees)
    │
    ▼
┌─────────────────────┐
│    CANCELLED ✓      │
└─────────────────────┘
```

---

## State Machine Diagram

### All States

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                            SAGA STATE MACHINE                                       │
└─────────────────────────────────────────────────────────────────────────────────────┘

                                    ┌─────────┐
                                    │ Initial │
                                    └────┬────┘
                                         │ TripBookingCreated
                                         ▼
                           ┌──────────────────────────────┐
                           │ AwaitingPaymentAuthorisation │
                           └──────────────┬───────────────┘
                        ┌─────────────────┴─────────────────┐
                        │                                   │
                PaymentAuthorised               PaymentAuthorisationFailed
                        │                                   │
                        ▼                                   ▼
           ┌────────────────────────┐              ┌────────────┐
           │ AwaitingOutboundFlight │              │   Failed   │
           └───────────┬────────────┘              └────────────┘
                       │
              OutboundFlightReserved ──────────────────┐
                       │                               │ Failed
                       ▼                               ▼
           ┌────────────────────────┐         ┌───────────────┐
           │  AwaitingReturnFlight  │         │ Compensating  │
           └───────────┬────────────┘         └───────┬───────┘
                       │                               │
              ReturnFlightReserved ────────────────────┤
                       │                               │
                       ▼                               │
           ┌────────────────────────┐                  │
           │     AwaitingHotel      │                  │
           └───────────┬────────────┘                  │
                       │                               │
                  HotelReserved ───────────────────────┤
                       │                               │
                       ▼                               │
           ┌────────────────────────┐                  │
           │  AwaitingHotelConfirm  │                  │
           └───────────┬────────────┘                  │
                       │                               │
                 HotelConfirmed ───────────────────────┤
                       │                               │
                       ▼                               │
           ┌─────────────────────────┐                 │
           │ AwaitingGroundTransport │◄── (if needed)  │
           └───────────┬─────────────┘                 │
                       │                               │
           GroundTransportReserved ────────────────────┤
                       │                               │
                       ▼                               │
           ┌────────────────────────┐                  │
           │   AwaitingInsurance    │◄── (if needed)   │
           └───────────┬────────────┘                  │
                       │                               │
                InsuranceIssued ───────────────────────┤
                       │                               │
                       ▼                               │
           ┌────────────────────────┐                  │
           │ AwaitingPaymentCapture │                  │
           └───────────┬────────────┘                  │
                       │                               │
               PaymentCaptured ◄───────────────────────┘
                       │                          (on failure)
                       ▼
               ┌─────────────┐
               │  Completed  │
               └─────────────┘
```

### Compensation Sub-States

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                        COMPENSATION SUB-STATES                                      │
└─────────────────────────────────────────────────────────────────────────────────────┘

                    ┌───────────────┐
                    │ Compensating  │
                    └───────┬───────┘
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ▼                   ▼                   ▼
┌───────────────┐  ┌────────────────┐  ┌────────────────────┐
│ Compensating  │  │ Compensating   │  │ Compensating       │
│ Insurance     │  │ GroundTransport│  │ Hotel              │
└───────┬───────┘  └────────┬───────┘  └─────────┬──────────┘
        │                   │                    │
        └───────────────────┼────────────────────┘
                            │
                            ▼
               ┌─────────────────────────┐
               │ CompensatingReturnFlight│
               └───────────┬─────────────┘
                           │
                           ▼
               ┌──────────────────────────┐
               │CompensatingOutboundFlight│
               └───────────┬──────────────┘
                           │
                           ▼
               ┌────────────────────────┐
               │   ReleasingPayment     │
               └───────────┬────────────┘
                           │
                           ▼
                    ┌────────────┐
                    │   Failed   │
                    └────────────┘
```

---

## Implementation Status

| Feature | Status | Notes |
|---------|--------|-------|
| Happy Path | ✅ Implemented | Full flow working |
| Optional Steps (IfElse) | ✅ Implemented | GroundTransport, Insurance |
| Payment Authorisation Failure | ✅ Implemented | Direct to Failed state |
| Outbound Flight Failure | ✅ Implemented | Compensation: Release payment |
| Return Flight Failure | ✅ Implemented | Compensation: Cancel outbound → Release |
| Hotel Reservation Failure | ✅ Implemented | Compensation: Cancel flights → Release |
| Hotel Confirmation Timeout | ✅ Implemented | Compensation chain (scheduler TODO) |
| Ground Transport Failure | ✅ Implemented | Compensation cascade |
| Insurance Failure | ✅ Implemented | Compensation cascade with IfElse |
| Payment Capture Failure | ✅ Implemented | Retry 3x + full compensation with IfElse |
| Timeouts (Quartz) | ✅ Implemented | All 8 timeouts with compensation |
| User Cancellation | ✅ Implemented | IsCancelledByUser flag, Cancelled terminal state |
| Post-Completion Refund | 🎯 TODO | RefundPayment flow |
| Manual Review State | 🎯 TODO | For edge cases |

---

## Testing Scenarios

All consumers have built-in failure simulation triggers. Use these in your HTTP requests to test compensation flows.

### Failure Triggers Summary

| Consumer | Trigger | Failure Event |
|----------|---------|---------------|
| `AuthorisePaymentConsumer` | `Amount == 0.01` | `PaymentAuthorisationFailed` |
| `ReserveOutboundFlightConsumer` | `FlightNumber.Contains("FAIL")` | `FlightReservationFailed` |
| `ReserveReturnFlightConsumer` | `FlightNumber.Contains("FAIL")` | `FlightReservationFailed` |
| `ReserveHotelConsumer` | `HotelName.Contains("FAIL")` | `HotelReservationFailed` |
| `ReserveGroundTransportConsumer` | `Type.Contains("FAIL")` | `GroundTransportReservationFailed` |
| `IssueInsuranceConsumer` | `CustomerName.Contains("FAIL")` | `InsuranceIssueFailed` |
| `CapturePaymentConsumer` | `Amount == 999.99` | `PaymentCaptureFailed` |

### Test HTTP Requests

All test requests are available in `Trip/Trip.API/Trip.API.http`:

1. **Happy Path** - Normal booking flow
2. **PaymentAuthorisationFailed** - `amount: 0.01`
3. **OutboundFlightReservationFailed** - `flightNumber: "FAIL"` (outbound)
4. **ReturnFlightReservationFailed** - `flightNumber: "FAIL"` (return)
5. **HotelReservationFailed** - `hotelName: "FAIL Hotel"`
6. **GroundTransportReservationFailed** - `type: "FAIL"`
7. **InsuranceIssueFailed** (with GroundTransport) - `customerName: "FAIL Smith"`
8. **InsuranceIssueFailed** (without GroundTransport) - `customerName: "FAIL Smith"`
9. **PaymentCaptureFailed** (with retry) - `amount: 999.99`

### Example: Test Outbound Flight Failure

```json
{
  "details": {
    "outboundFlight": {
      "flightNumber": "FAIL",  // <-- Triggers FlightReservationFailed
      ...
    }
  }
}
```

**Expected compensation:** `ReleasePayment`

### Example: Test Hotel Failure

```json
{
  "details": {
    "hotel": {
      "hotelName": "FAIL Hotel",  // <-- Triggers HotelReservationFailed
      ...
    }
  }
}
```

**Expected compensation:** `CancelReturnFlight → CancelOutboundFlight → ReleasePayment`

---

## References

- [MassTransit State Machine Saga](https://masstransit.io/documentation/patterns/saga/state-machine)
- [SAGA Pattern - Microservices.io](https://microservices.io/patterns/data/saga.html)
- [Compensating Transaction Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/compensating-transaction)
