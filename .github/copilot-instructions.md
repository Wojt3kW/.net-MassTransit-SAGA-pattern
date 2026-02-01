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

- **.NET 10** - Target framework
- **C# 14** - Programming language
- **MS SQL Server** - Database (separate database per microservice)
- **RabbitMQ** - Message broker
- **MassTransit** - Message bus abstraction and SAGA orchestration
- **MediatR** - CQRS implementation
- **Entity Framework Core** - ORM

### Architecture Patterns

- **Clean Architecture** - Layered architecture with dependency inversion
- **Vertical Slice Architecture** - Feature-based code organisation
- **CQRS** - Command Query Responsibility Segregation
- **Event-Driven Architecture** - Asynchronous communication
- **Orchestration SAGA Pattern** - Distributed transaction management
- **Repository Pattern** - Data access abstraction
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

## Solution Structure

```
SAGA pattern.sln
├── SAGA pattern.AppHost/              # .NET Aspire orchestrator
│   └── Program.cs
├── SAGA pattern.ServiceDefaults/      # Shared service configuration
│   ├── Extensions.cs
│   └── Settings/
│       ├── ApiSettings.cs
│       └── ConnectionStrings.cs
│
├── FrontDashboard/                    # Angular monitoring dashboard
│   └── src/app/
│       ├── core/                      # Shared services and models
│       │   ├── models/
│       │   └── services/
│       └── features/                  # Feature components
│           ├── dashboard/
│           ├── create-trip/
│           └── trip-details/
│
├── Trip/
│   ├── Trip.Domain/                   # Entities (zero dependencies)
│   │   └── Entities/
│   │       └── TripBooking.cs
│   ├── Trip.Application/              # Use cases, repository interfaces
│   │   └── Abstractions/
│   │       └── ITripRepository.cs
│   ├── Trip.Infrastructure/           # Persistence implementations
│   │   ├── Persistence/
│   │   │   └── TripDbContext.cs
│   │   └── Repositories/
│   │       └── TripRepository.cs
│   ├── Trip.API/                      # Entry point, starts SAGA
│   │   ├── Features/
│   │   │   ├── CreateTrip/
│   │   │   │   ├── CreateTripCommand.cs
│   │   │   │   ├── CreateTripCommandValidator.cs
│   │   │   │   ├── CreateTripEndpoint.cs
│   │   │   │   └── CreateTripHandler.cs
│   │   │   ├── GetTrip/
│   │   │   │   ├── GetTripEndpoint.cs
│   │   │   │   ├── GetTripHandler.cs
│   │   │   │   └── GetTripQuery.cs
│   │   │   ├── CancelTrip/
│   │   │   │   ├── CancelTripCommand.cs
│   │   │   │   ├── CancelTripEndpoint.cs
│   │   │   │   └── CancelTripHandler.cs
│   │   │   ├── IEndpoint.cs
│   │   │   ├── EndpointExtensions.cs
│   │   │   └── ValidationBehavior.cs
│   │   ├── Consumers/                 # Event consumers updating TripBookings
│   │   │   ├── OutboundFlightReservedConsumer.cs
│   │   │   ├── ReturnFlightReservedConsumer.cs
│   │   │   ├── HotelReservedConsumer.cs
│   │   │   ├── InsuranceIssuedConsumer.cs
│   │   │   ├── PaymentCapturedConsumer.cs
│   │   │   └── TripBookingCompletedConsumer.cs
│   │   └── Program.cs
│   └── Trip.Contracts/                # Shared DTOs, Commands, Events
│       ├── Commands/
│       │   ├── CreateTripBooking.cs
│       │   └── CancelTripBooking.cs
│       ├── Events/
│       │   ├── TripBookingCreated.cs
│       │   ├── TripBookingCompleted.cs
│       │   ├── TripBookingFailed.cs
│       │   └── TripBookingCancelled.cs
│       └── DTOs/
│           ├── TripBookingResponse.cs
│           └── TripBookingStatusResponse.cs
│
├── TripBooking.Saga/
│   ├── TripBooking.Saga/              # SAGA State Machine library
│   │   ├── StateMachines/
│   │   │   └── TripBookingStateMachine.cs
│   │   ├── States/
│   │   │   └── TripBookingSagaState.cs
│   │   └── Persistence/
│   │       └── TripBookingSagaDbContext.cs
│   └── TripBooking.Saga.API/          # SAGA host & monitoring endpoints
│
├── FlightBooking/
│   ├── FlightBooking.Domain/
│   │   └── Entities/
│   │       └── FlightReservation.cs
│   ├── FlightBooking.Application/
│   │   └── Abstractions/
│   │       └── IFlightReservationRepository.cs
│   ├── FlightBooking.Infrastructure/
│   │   ├── Persistence/
│   │   └── Repositories/
│   ├── FlightBooking.API/
│   │   ├── Features/
│   │   │   ├── ReserveOutboundFlight/
│   │   │   ├── ReserveReturnFlight/
│   │   │   └── CancelFlight/
│   │   └── Program.cs
│   └── FlightBooking.Contracts/
│       ├── Commands/
│       └── Events/
│
├── HotelBooking/
│   ├── HotelBooking.Domain/
│   │   └── Entities/
│   │       └── HotelReservation.cs
│   ├── HotelBooking.Application/
│   │   └── Abstractions/
│   │       └── IHotelReservationRepository.cs
│   ├── HotelBooking.Infrastructure/
│   │   ├── Persistence/
│   │   └── Repositories/
│   ├── HotelBooking.API/
│   │   ├── Features/
│   │   │   ├── ReserveHotel/
│   │   │   ├── ConfirmHotel/
│   │   │   └── CancelHotel/
│   │   └── Program.cs
│   └── HotelBooking.Contracts/
│       ├── Commands/
│       └── Events/
│
├── GroundTransport/
│   ├── GroundTransport.Domain/
│   │   └── Entities/
│   │       └── TransportReservation.cs
│   ├── GroundTransport.Application/
│   │   └── Abstractions/
│   │       └── ITransportReservationRepository.cs
│   ├── GroundTransport.Infrastructure/
│   │   ├── Persistence/
│   │   └── Repositories/
│   ├── GroundTransport.API/
│   │   ├── Features/
│   │   │   ├── ReserveGroundTransport/
│   │   │   └── CancelGroundTransport/
│   │   └── Program.cs
│   └── GroundTransport.Contracts/
│       ├── Commands/
│       └── Events/
│
├── Insurance/
│   ├── Insurance.Domain/
│   │   └── Entities/
│   │       └── InsurancePolicy.cs
│   ├── Insurance.Application/
│   │   └── Abstractions/
│   │       └── IInsurancePolicyRepository.cs
│   ├── Insurance.Infrastructure/
│   │   ├── Persistence/
│   │   └── Repositories/
│   ├── Insurance.API/
│   │   ├── Features/
│   │   │   ├── IssueInsurance/
│   │   │   └── CancelInsurance/
│   │   └── Program.cs
│   └── Insurance.Contracts/
│       ├── Commands/
│       └── Events/
│
├── Payment/
│   ├── Payment.Domain/
│   │   └── Entities/
│   │       └── PaymentTransaction.cs
│   ├── Payment.Application/
│   │   └── Abstractions/
│   │       └── IPaymentTransactionRepository.cs
│   ├── Payment.Infrastructure/
│   │   ├── Persistence/
│   │   └── Repositories/
│   ├── Payment.API/
│   │   ├── Features/
│   │   │   ├── AuthorisePayment/
│   │   │   ├── CapturePayment/
│   │   │   ├── ReleasePayment/
│   │   │   └── RefundPayment/
│   │   └── Program.cs
│   └── Payment.Contracts/
│       ├── Commands/
│       └── Events/
│
└── Notification/
    ├── Notification.Domain/
    │   └── Entities/
    │       └── NotificationRecord.cs
    ├── Notification.Application/
    │   └── Abstractions/
    │       └── INotificationRepository.cs
    ├── Notification.Infrastructure/
    │   ├── Persistence/
    │   └── Repositories/
    ├── Notification.API/
    │   ├── Features/
    │   │   ├── SendBookingConfirmation/
    │   │   ├── SendBookingFailure/
    │   │   └── SendCancellation/
    │   └── Program.cs
    └── Notification.Contracts/
        ├── Commands/
        └── Events/
```

---

## Microservices Architecture

### 1. Trip.API (Entry Point / BFF)

**Responsibility:** Public REST API, starts the SAGA, exposes booking status

**Database:** `TripDb`

**Endpoints:**

- `POST /api/trips` - Create new trip booking
- `GET /api/trips/{id}` - Get trip status
- `POST /api/trips/{id}/cancel` - Request cancellation

---

### 2. TripBooking.Saga.API (Orchestrator & Monitoring)

**Responsibility:** Orchestrates the entire booking process, provides monitoring and management endpoints

**Database:** `TripBookingSagaDb`

**Persistence:** `TripBookingSagaState` table via EF Core

**Key Features:**

- State machine implementation
- Timeout scheduling (Quartz)
- Compensation orchestration
- Manual review escalation

**Endpoints:**

- `GET /api/sagas/{tripId}` - Get detailed saga state by trip ID
- `GET /api/sagas` - List all sagas with pagination and filtering (by state, customerId)

**Architecture:**

- Uses Vertical Slice Architecture with Features/
- MediatR for CQRS queries
- Direct DbContext access for read-only queries
- No business logic - only orchestration and monitoring

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

| Operation             | Timeout | Action on Timeout                      |
| --------------------- | ------- | -------------------------------------- |
| Payment Authorisation | 30s     | Fail, no compensation needed           |
| Outbound Flight       | 60s     | Release payment                        |
| Return Flight         | 60s     | Cancel outbound, release payment       |
| Hotel Reservation     | 60s     | Cancel flights, release payment        |
| Hotel Confirmation    | 15min   | Cancel hotel, flights, release payment |
| Ground Transport      | 60s     | Cancel hotel, flights, release payment |
| Insurance             | 30s     | Full compensation cascade              |
| Payment Capture       | 30s     | Retry 3x, then compensation            |
| User Inactivity       | 30min   | Auto-cancel with compensation          |

---

## Compensation Matrix

| Failed Step                | Compensation Required                                                                 |
| -------------------------- | ------------------------------------------------------------------------------------- |
| Return Flight              | Cancel outbound flight → Release payment                                              |
| Hotel                      | Cancel return flight → Cancel outbound flight → Release payment                       |
| Hotel Confirmation Timeout | Cancel hotel → Cancel flights → Release payment                                       |
| Ground Transport           | Cancel hotel → Cancel flights → Release payment                                       |
| Insurance                  | Cancel transport → Cancel hotel → Cancel flights → Release payment                    |
| Payment Capture            | Cancel insurance → Cancel transport → Cancel hotel → Cancel flights → Release payment |
| User Cancellation          | Compensate all completed steps in reverse order                                       |

---

## Coding Standards

### Code Documentation

- **Add XML comments to classes** - Use `/// <summary>` to describe the purpose of each class
- **Add XML comments to properties** - Use `/// <summary>` to explain what each property represents
- Keep comments **short and concise** - One line is usually sufficient
- Use proper English grammar in comments

```csharp
// ✅ Good - Short, descriptive comments
/// <summary>
/// Represents a complete travel booking containing all trip components.
/// </summary>
public class TripBooking
{
    /// <summary>Unique identifier of the trip booking.</summary>
    public Guid Id { get; set; }

    /// <summary>Customer who made the booking.</summary>
    public Guid CustomerId { get; set; }

    /// <summary>Current status of the trip booking process.</summary>
    public TripStatus Status { get; set; }
}
```

### File Organisation (Single Responsibility)

- **One type per file** - Each class, record, enum, or interface MUST be in its own separate file
- File name MUST match the type name (e.g., `TripStatus.cs` for `enum TripStatus`)
- This applies to all layers: Domain, Application, Infrastructure, API, and Contracts

### Prefer Records Over Classes

- **Use `record` for immutable data types** - DTOs, Commands, Events, Value Objects
- **Use `class` only when** mutability or identity semantics are required (e.g., Entities with EF Core tracking)
- Records provide: immutability, value equality, `with` expressions, and concise syntax

```csharp
// ✅ Good - Records for immutable data
public record CreateTripCommand(Guid CustomerId, string Email, TripDetailsDto Details);
public record TripBookingResponse(Guid TripId, string Status, DateTime CreatedAt);
public record FlightDetails(string Origin, string Destination, DateTime DepartureDate);

// ✅ Good - Class for mutable entity with identity
public class TripBooking
{
    public Guid Id { get; set; }
    public TripStatus Status { get; set; }
    // ... mutable properties tracked by EF Core
}
```

### Naming Conventions

- PascalCase for public members
- camelCase for private fields with underscore prefix (`_fieldName`)
- Async suffix for async methods (`GetTripAsync`)

### Code Organisation

- **Do NOT use `#region`** - Regions hide code and make navigation harder
- Use blank lines and comments to separate logical sections instead
- Keep classes focused and small - if you need regions, the class is likely too large
- **Always use braces `{}`** for `if`, `else`, `for`, `foreach`, `while`, `do`, `using`, etc.
- **Prefer `is null`** over `== null` for null checks
- **Prefer `is not null`** over `!= null` for non-null checks
- Pattern matching is more readable and avoids operator overloading issues
- **Always use `DateTime.UtcNow`** - Never use `DateTime.Now` to avoid timezone issues
- **Store all timestamps in UTC** - Convert to local time only for display
- **Use `datetime2(3)` precision** - Millisecond precision is sufficient for most use cases
- Configure precision in DbContext using `.HasPrecision(3)`

### Project Structure (Clean Architecture + Vertical Slice + REPR Pattern)

Each microservice follows Clean Architecture with 5 projects:

```
ServiceName/
├── ServiceName.Domain/            # Entities (ZERO dependencies)
│   └── Entities/
│       └── Entity.cs
├── ServiceName.Application/       # Use cases, interfaces (depends on Domain)
│   └── Abstractions/
│       └── IEntityRepository.cs
├── ServiceName.Infrastructure/    # Implementations (depends on Domain + Application)
│   ├── Persistence/
│   │   └── ServiceNameDbContext.cs
│   └── Repositories/
│       └── EntityRepository.cs
├── ServiceName.API/               # Presentation (depends on Application + Infrastructure*)
│   ├── Features/
│   │   └── FeatureName/
│   │       ├── FeatureNameEndpoint.cs     # Minimal API endpoint (REPR)
│   │       ├── FeatureNameCommand.cs      # Request DTO
│   │       ├── FeatureNameHandler.cs      # MediatR handler
│   │       ├── FeatureNameValidator.cs    # FluentValidation
│   │       └── FeatureNameResponse.cs     # Response DTO
│   └── Program.cs
└── ServiceName.Contracts/         # Shared message contracts
    ├── Commands/
    └── Events/
```

_\* API references Infrastructure only for Composition Root (DI registration)_

### Clean Architecture Dependency Rules

```
┌─────────────────────────────────────────────┐
│                   API                        │  ← Composition Root
├─────────────────────────────────────────────┤
│              Infrastructure                  │  ← Implements Application interfaces
├─────────────────────────────────────────────┤
│               Application                    │  ← Defines interfaces (ports)
├─────────────────────────────────────────────┤
│                 Domain                       │  ← Pure business entities
└─────────────────────────────────────────────┘

Dependency Direction: Domain ← Application ← Infrastructure
                                          ↖        ↗
                                            API
```

### REPR Pattern Example

```csharp
// CreateTripEndpoint.cs
public class CreateTripEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/trips", async (CreateTripCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return Results.Created($"/api/trips/{result.TripId}", result);
        })
        .WithName("CreateTrip")
        .WithOpenApi();
    }
}
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

## Local Development

### Run with Aspire

```powershell
dotnet run --project "SAGA pattern.AppHost"
```

Aspire Dashboard: `http://localhost:15888`

### Required Infrastructure (via Aspire)

- SQL Server 2022
- RabbitMQ with management plugin

### EF Core Migrations

Each microservice has an `add-migration.bat` script in its Infrastructure project for creating migrations.

**Location:** `{ServiceName}/{ServiceName}.Infrastructure/add-migration.bat`

**Usage:**

```powershell
# Navigate to Infrastructure project and run the script with migration name
cd Trip\Trip.Infrastructure
.\add-migration.bat my_migration_name
```

**Available migration scripts:**

- `Trip/Trip.Infrastructure/add-migration.bat`
- `FlightBooking/FlightBooking.Infrastructure/add-migration.bat`
- `HotelBooking/HotelBooking.Infrastructure/add-migration.bat`
- `GroundTransport/GroundTransport.Infrastructure/add-migration.bat`
- `Insurance/Insurance.Infrastructure/add-migration.bat`
- `Payment/Payment.Infrastructure/add-migration.bat`
- `Notification/Notification.Infrastructure/add-migration.bat`

**Manual migration command:**

```powershell
cd {ServiceName}\{ServiceName}.Infrastructure
dotnet ef migrations add {migration_name} --startup-project ..\{ServiceName}.API --output-dir Migrations
```

**Apply migrations:** Migrations are applied automatically at application startup via the `MigrateDatabaseAsync<TDbContext>()` extension method in `Extensions.cs`.

---

## Configuration & Settings

### ApiSettings Pattern

All microservices use centralized configuration via `ApiSettings` record:

**Injecting settings in services:**

```csharp
public class SomeService(ApiSettings settings)
{
    private readonly string _connectionString = settings.ConnectionStrings.SqlServer;
}
```

### Health Checks

Health checks are automatically configured by `AddServiceDefaults()` for:

- **SQL Server** - Verifies database connectivity (`SELECT 1`)
- **RabbitMQ** - Verifies message broker connectivity
- **MassTransit** - Built-in health checks for consumers

**Endpoints:**

- `/health` - All health checks (readiness probe)
- `/alive` - Basic liveness check

### CRUD Endpoints

Each microservice exposes standard CRUD endpoints for testing:

| Endpoint                           | Description                            |
| ---------------------------------- | -------------------------------------- |
| `GET /api/{resource}`              | List all with filtering and pagination |
| `GET /api/{resource}/{id:guid}`    | Get by ID                              |
| `DELETE /api/{resource}/{id:guid}` | Delete by ID                           |

**Query Parameters for GET all:**

- `tripId` - Filter by trip booking ID
- `status` - Filter by status (service-specific)
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

### SAGA Monitoring Endpoints (TripBooking.Saga.API)

Special read-only endpoints for SAGA monitoring and troubleshooting:

| Endpoint                       | Description                        |
| ------------------------------ | ---------------------------------- |
| `GET /api/sagas/{tripId:guid}` | Get detailed saga state by trip ID |
| `GET /api/sagas`               | List all sagas with pagination     |

**Query Parameters for GET /api/sagas:**

- `state` - Filter by current state (e.g., "Completed", "Failed", "AwaitingPaymentAuthorisation")
- `customerId` - Filter by customer ID
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

---

## Angular Dashboard (FrontDashboard)

A web-based monitoring dashboard for real-time SAGA state visualization and trip booking management.

### Technology Stack

| Category | Technology |
|----------|------------|
| Framework | Angular 21 (CLI) |
| UI Library | Angular Material 21 |
| Components | Standalone components |
| Styling | SCSS |
| Real-time | Server-Sent Events (SSE) |
| Forms | Reactive Forms with validation |

### Project Structure

```
FrontDashboard/
├── src/
│   ├── app/
│   │   ├── core/                      # Shared services and models
│   │   │   ├── models/
│   │   │   │   └── trip.models.ts     # TypeScript interfaces and constants
│   │   │   └── services/
│   │   │       └── trip.service.ts    # HTTP client and SSE handling
│   │   ├── features/                  # Feature components
│   │   │   ├── dashboard/
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   ├── dashboard.component.html
│   │   │   │   └── dashboard.component.scss
│   │   │   ├── create-trip/
│   │   │   │   ├── create-trip.component.ts
│   │   │   │   ├── create-trip.component.html
│   │   │   │   └── create-trip.component.scss
│   │   │   └── trip-details/
│   │   │       ├── trip-details.component.ts
│   │   │       ├── trip-details.component.html
│   │   │       └── trip-details.component.scss
│   │   ├── app.ts                     # Root component
│   │   ├── app.routes.ts              # Routing configuration
│   │   └── app.config.ts              # App configuration with providers
│   ├── environments/
│   │   └── environment.ts             # API base URLs
│   └── styles.scss                    # Global styles
├── angular.json
└── package.json
```

### Features

#### Dashboard Component
- Real-time SAGA states table with color-coded status chips
- Statistics cards (total, in-progress, completed, failed)
- Toggle between SSE (real-time) and polling modes
- Connection status indicator
- Quick navigation to trip details or create new trip

#### Create Trip Component
- Multi-step wizard form using Angular Material Stepper
- 5 steps: Customer → Flights → Hotel → Options/Payment → Review
- Built-in failure simulation hints (e.g., amount=0.01 for PaymentAuthorisationFailed)
- Reactive forms with validation
- Optional ground transport and insurance toggles

#### Trip Details Component
- SAGA progress timeline with animated active step
- Header card with current state and cancel button
- Tabbed view for SAGA State, Flights, Hotel, and Payment details
- Related reservation data fetched from individual microservices
- Auto-polling for real-time updates

### API Integration

The Angular dashboard communicates with multiple microservices via HTTPS:

| Service | Port | Usage |
|---------|------|-------|
| Trip.API | 7172 | Create trips, cancel trips |
| TripBooking.Saga.API | 7276 | Get SAGA states, SSE streaming |
| FlightBooking.API | 7246 | Get flight reservations |
| HotelBooking.API | 7081 | Get hotel reservations |
| Payment.API | 7123 | Get payment transactions |

### Server-Sent Events (SSE)

The dashboard supports real-time updates via SSE:

**Backend Endpoint:** `GET /api/sagas/stream`

```csharp
// TripBooking.Saga.API/Features/StreamSagas/StreamSagasEndpoint.cs
app.MapGet("/api/sagas/stream", async (TripBookingSagaDbContext db, CancellationToken ct) =>
{
    async IAsyncEnumerable<SagaSummaryResponse> StreamSagas([EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var sagas = await db.TripBookingSagaStates
                .OrderByDescending(s => s.CreatedAt)
                .Take(50)
                .ToListAsync(ct);
            
            foreach (var saga in sagas)
            {
                yield return MapToResponse(saga);
            }
            
            await Task.Delay(2000, ct);
        }
    }
    
    return TypedResults.ServerSentEvents(StreamSagas(ct), eventType: "saga-update");
})
.WithName("StreamSagas");
```

**Frontend Service:**

```typescript
// TripService - SSE handling
startSagaSSE(): void {
    this.eventSource = new EventSource(`${this.sagaApiUrl}/api/sagas/stream`);
    this.eventSource.addEventListener('saga-update', (event) => {
        const saga = JSON.parse(event.data);
        // Buffer and debounce updates
    });
}
```

### Angular Coding Standards

#### Component Structure
- **Separate files** - Each component has `.ts`, `.html`, and `.scss` files
- **Standalone components** - No NgModules, use `imports` in component decorator
- Use `templateUrl` and `styleUrl` instead of inline template/styles

```typescript
// ✅ Good - Separate files with templateUrl/styleUrl
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, ...],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent { }
```

#### Signals
- Use Angular signals for reactive state management
- Use `computed()` for derived state
- Use `signal()` for simple reactive values

```typescript
// ✅ Good - Signals for state
loading = signal(true);
sagaStates = signal<SagaSummaryResponse[]>([]);
totalCount = computed(() => this.sagaStates().length);
```

#### Dependency Injection
- Use `inject()` function instead of constructor injection

```typescript
// ✅ Good - inject() function
private tripService = inject(TripService);
private router = inject(Router);
```

### Running the Dashboard

```powershell
cd FrontDashboard
npm install
npm start
```

Dashboard URL: `http://localhost:4200`

**Note:** The backend microservices must be running (via Aspire) for the dashboard to function properly.

---

## Additional Guidelines

1. **Idempotency:** All commands must be idempotent
2. **Correlation:** Use CorrelationId across all operations
3. **Eventual Consistency:** Design for eventual consistency, not distributed transactions
4. **Circuit Breaker:** Implement circuit breaker pattern for external service calls
5. **Retry Policy:** Use exponential backoff for retries
6. **Dead Letter Queue:** Configure DLQ for unprocessable messages
7. **Feature Flags:** Consider feature flags for gradual rollout
8. **Configuration:** Use `ApiSettings` pattern for all configuration needs - inject via constructor
9. **Documentation Lookup:** Always use `io.github.upstash/context7` MCP server to fetch up-to-date documentation for libraries before implementing features

---

## References

- [MassTransit Documentation](https://masstransit.io/)
- [SAGA Pattern](https://microservices.io/patterns/data/saga.html)
- [Vertical Slice Architecture](https://www.jimmybogard.com/vertical-slice-architecture/)
- [Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html)
