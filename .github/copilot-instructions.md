# Copilot Instructions for Travel Booking Platform

## Project Overview

This is an **enterprise-grade Travel Booking Platform** built with .NET microservices architecture, implementing the **Orchestration-based SAGA pattern** using MassTransit.

### Domain Description

The platform enables users to book complete travel packages including:
- Round-trip flights (outbound + return)
- Hotel accommodation
- Ground transportation (airport transfers, car rental)
- Travel insurance
- Payment processing (authorize → capture → release)

This is a complex business process that can span several hours, with multiple failure points requiring compensation and manual intervention capabilities.

---

## Technology Stack

### Core Technologies
- **.NET 9** - Target framework
- **C# 13** - Programming language
- **MS SQL Server** - Database (separate database per microservice)
- **RabbitMQ** - Message broker
- **MassTransit** - Message bus abstraction and SAGA orchestration
- **MediatR** - CQRS implementation
- **Entity Framework Core** - ORM

### Architecture Patterns
- **Vertical Slice Architecture** - Feature-based code organisation
- **CQRS** - Command Query Responsibility Segregation
- **Event-Driven Architecture** - Asynchronous communication
- **Orchestration SAGA Pattern** - Distributed transaction management
- **Inbox Pattern** - Message idempotency and exactly-once delivery
- **Outbox Pattern** - At-least-once delivery guarantee

### API & Documentation
- **Minimal API** - Lightweight endpoints
- **ASP.NET Core OpenAPI** - Built-in OpenAPI/Swagger documentation
- **FluentValidation** - Request validation with MediatR pipeline behaviour
- **Health Checks** - Infrastructure health monitoring

### Observability Stack
- **Serilog + Kibana** - Structured logging
- **OpenTelemetry + Jaeger** - Distributed tracing
- **OpenTelemetry + Prometheus + Grafana** - Metrics and monitoring
- **.NET Aspire** - Service discovery, observability, and local orchestration

### Infrastructure & Deployment
- **Docker Compose** - Local development
- **Kubernetes** - Production deployment with scaling and high availability
- **YARP** - API Gateway / Reverse proxy
- **Nginx Ingress Controller** - Load balancing in Kubernetes
- **cert-manager** - TLS certificate management

### Testing
- **xUnit** - Test framework
- **NSubstitute** - Mocking library
- **Testcontainers** - Integration and E2E testing with real dependencies
- **FluentAssertions** - Assertion library

---

## Microservices Architecture

### 1. Trip.API (Entry Point / BFF)
**Responsibility:** Public REST API, starts the SAGA, exposes booking status

**Database:** `TripDb`

**Endpoints:**
- `POST /trips` - Create new trip booking
- `GET /trips/{id}` - Get trip status
- `POST /trips/{id}/cancel` - Request cancellation

---

### 2. TripBooking.Saga (Orchestrator)
**Responsibility:** Orchestrates the entire booking process, contains only workflow logic and decisions

**Database:** `TripBookingSagaDb`

**Persistence:** `TripBookingSagaState` table via EF Core

**Key Features:**
- State machine implementation
- Timeout scheduling (Quartz)
- Compensation orchestration
- Manual review escalation

---

### 3. FlightBooking.API
**Responsibility:** Flight reservations (outbound and return)

**Database:** `FlightBookingDb`

**Complexity:**
- Separate outbound and return flights
- Multiple carriers support
- Partial reservation handling

**Commands:**
- `ReserveOutboundFlight`
- `ReserveReturnFlight`
- `CancelFlight`

**Events:**
- `OutboundFlightReserved`
- `ReturnFlightReserved`
- `FlightReservationPartiallyFailed`
- `FlightReservationFailed`
- `FlightCancelled`

---

### 4. HotelBooking.API
**Responsibility:** Hotel accommodation reservations

**Database:** `HotelBookingDb`

**Complexity:**
- Various cancellation policies
- Confirmation timeout handling
- Partial payment support

**Commands:**
- `ReserveHotel`
- `ConfirmHotel`
- `CancelHotel`

**Events:**
- `HotelReserved`
- `HotelConfirmed`
- `HotelConfirmationExpired`
- `HotelReservationFailed`
- `HotelCancelled`

---

### 5. GroundTransport.API
**Responsibility:** Airport transfers and car rentals

**Database:** `GroundTransportDb`

**Complexity:**
- Optional service (SAGA must handle absence)
- Airport-to-hotel transfers
- Car rental bookings

**Commands:**
- `ReserveGroundTransport`
- `CancelGroundTransport`

**Events:**
- `GroundTransportReserved`
- `GroundTransportReservationFailed`
- `GroundTransportCancelled`

---

### 6. Insurance.API
**Responsibility:** Travel insurance policies

**Database:** `InsuranceDb`

**Complexity:**
- Insurance activated only after complete booking
- Requires flight and hotel booking references

**Commands:**
- `IssueInsurance`
- `CancelInsurance`

**Events:**
- `InsuranceIssued`
- `InsuranceIssueFailed`
- `InsuranceCancelled`

---

### 7. Payment.API
**Responsibility:** Payment processing with two-phase commit pattern

**Database:** `PaymentDb`

**Process Flow:**
1. **Authorise** - Block funds on card
2. **Capture** - Charge after successful booking
3. **Release** - Unblock funds on failure

**Commands:**
- `AuthorisePayment`
- `CapturePayment`
- `ReleasePayment`
- `RefundPayment`

**Events:**
- `PaymentAuthorised`
- `PaymentAuthorisationFailed`
- `PaymentCaptured`
- `PaymentCaptureFailed`
- `PaymentReleased`
- `PaymentRefunded`

---

### 8. Notification.API
**Responsibility:** Customer notifications (asynchronous side-effect)

**Database:** `NotificationDb`

**Channels:**
- Email
- SMS
- Push notifications

**Note:** Does NOT affect SAGA state - fire and forget

---

## SAGA States

```
Initial
├── AwaitingPaymentAuthorisation
├── AwaitingOutboundFlight
├── AwaitingReturnFlight
├── AwaitingHotel
├── AwaitingHotelConfirmation
├── AwaitingGroundTransport
├── AwaitingInsurance
├── AwaitingPaymentCapture
├── AwaitingFinalConfirmation
├── Completed ✓
├── Failed ✗
├── Compensating
│   ├── CompensatingInsurance
│   ├── CompensatingGroundTransport
│   ├── CompensatingHotel
│   ├── CompensatingReturnFlight
│   ├── CompensatingOutboundFlight
│   └── ReleasingPayment
├── Cancelled
├── TimedOut
└── ManualReviewRequired
```

---

## Timeout Configuration

| Operation | Timeout | Action on Timeout |
|-----------|---------|-------------------|
| Payment Authorisation | 30s | Fail, no compensation needed |
| Outbound Flight | 60s | Release payment |
| Return Flight | 60s | Cancel outbound, release payment |
| Hotel Reservation | 60s | Cancel flights, release payment |
| Hotel Confirmation | 15min | Cancel hotel, flights, release payment |
| Ground Transport | 60s | Cancel hotel, flights, release payment |
| Insurance | 30s | Full compensation cascade |
| Payment Capture | 30s | Retry 3x, then compensation |
| User Inactivity | 30min | Auto-cancel with compensation |

---

## Compensation Matrix

| Failed Step | Compensation Required |
|-------------|----------------------|
| Return Flight | Cancel outbound flight → Release payment |
| Hotel | Cancel return flight → Cancel outbound flight → Release payment |
| Hotel Confirmation Timeout | Cancel hotel → Cancel flights → Release payment |
| Ground Transport | Cancel hotel → Cancel flights → Release payment |
| Insurance | Cancel transport → Cancel hotel → Cancel flights → Release payment |
| Payment Capture | Cancel insurance → Cancel transport → Cancel hotel → Cancel flights → Release payment |
| User Cancellation | Compensate all completed steps in reverse order |

---

## Coding Standards

### Naming Conventions
- PascalCase for public members
- camelCase for private fields with underscore prefix (`_fieldName`)
- Async suffix for async methods (`GetTripAsync`)

### Project Structure (Vertical Slice)
```
ServiceName.API/
├── Features/
│   ├── FeatureName/
│   │   ├── Command.cs
│   │   ├── CommandHandler.cs
│   │   ├── CommandValidator.cs
│   │   ├── Query.cs
│   │   ├── QueryHandler.cs
│   │   └── Endpoint.cs
├── Domain/
│   ├── Entities/
│   └── ValueObjects/
├── Infrastructure/
│   ├── Persistence/
│   │   ├── DbContext.cs
│   │   ├── Configurations/
│   │   └── Migrations/
│   ├── Messaging/
│   │   ├── Consumers/
│   │   └── Publishers/
│   └── Services/
├── Contracts/
│   ├── Commands/
│   └── Events/
└── Program.cs
```

### MassTransit Conventions
- Commands: `I{Action}{Entity}` (e.g., `IReserveOutboundFlight`)
- Events: `I{Entity}{Action}` (e.g., `IOutboundFlightReserved`)
- Consumers: `{Event}Consumer` or `{Command}Consumer`
- State Machine: `{Process}StateMachine`

### Validation
- Use FluentValidation for all commands
- Implement `IPipelineBehavior<TRequest, TResponse>` for validation pipeline
- Return problem details for validation errors

### Error Handling
- Use Result pattern for operation outcomes
- Implement global exception handling middleware
- Log all exceptions with correlation ID

### Testing
- Unit tests for handlers and validators
- Integration tests with Testcontainers
- Use `Arrange-Act-Assert` pattern
- Test file naming: `{ClassName}Tests.cs`

---

## Docker & Local Development

### Required Services
- SQL Server 2022
- RabbitMQ with management plugin
- Jaeger
- Prometheus
- Grafana
- Kibana + Elasticsearch

### Aspire Integration
Use .NET Aspire for local development orchestration with automatic service discovery and health monitoring.

---

## Additional Guidelines

1. **Idempotency:** All commands must be idempotent
2. **Correlation:** Use CorrelationId across all operations
3. **Eventual Consistency:** Design for eventual consistency, not distributed transactions
4. **Circuit Breaker:** Implement circuit breaker pattern for external service calls
5. **Retry Policy:** Use exponential backoff for retries
6. **Dead Letter Queue:** Configure DLQ for unprocessable messages
7. **Feature Flags:** Consider feature flags for gradual rollout

---

## References

- [MassTransit Documentation](https://masstransit.io/)
- [SAGA Pattern](https://microservices.io/patterns/data/saga.html)
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)
- [Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)
