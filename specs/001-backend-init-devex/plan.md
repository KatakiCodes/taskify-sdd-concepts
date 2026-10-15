# Implementation Plan: Backend Initialization & Developer Experience (DevEx) Foundation

**Branch**: `001-backend-init-devex` | **Date**: 2026-10-14 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [specs/001-backend-init-devex/spec.md](spec.md)

## Summary

Establish the initial backend architecture and developer experience (DevEx) for TaskFy. The solution establishes a partitioned root workspace (`src/Backend` and `src/Frontend`), implementing Clean Architecture, Domain-Driven Design (DDD), and CQRS with vertical slicing. In accordance with user directives, persistence uses Entity Framework Core 10 with SQLite, mapping uses Mapster, documentation uses Swagger/OpenAPI, and CQRS uses a lightweight, in-house hardcoded dispatcher (without MediatR). Testing is fully configured with xUnit, Moq, and an automated integration test harness (`WebApplicationFactory` with in-memory SQLite fixtures) providing a turnkey happy-path integration test baseline and RFC 7807 Problem Details error translation.

## Technical Context

**Language/Version**: C# / .NET 10 (ASP.NET Core Web API)

**Primary Dependencies**:
- `Microsoft.EntityFrameworkCore.Sqlite` (v10.x)
- `Mapster` & `Mapster.DependencyInjection`
- `Swashbuckle.AspNetCore` (v7.x / OpenAPI)
- In-house CQRS Dispatcher (`IDispatcher`, `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler` in `TaskFy.Application` - zero MediatR dependency)

**Storage**: SQLite (`Microsoft.Data.Sqlite`, `Microsoft.EntityFrameworkCore.Sqlite`) with in-memory SQLite fixtures for isolated integration testing.

**Testing**:
- Unit Tests: `xunit`, `moq`, `xunit.runner.visualstudio`
- Integration Tests: `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`), in-memory SQLite fixture

**Target Platform**: Cross-platform (.NET 10 on Windows, Linux, and macOS)

**Project Type**: Clean Architecture Web API with CQRS & Vertical Slice baseline

**Performance Goals**:
- Local health check probe response: < 200ms
- Full test suite execution: < 5 seconds
- Cold build time: < 10 seconds

**Constraints**:
- Strict One-Type-Per-File constraint across all backend projects.
- Result Pattern (`Result<T>` and `Result`) for all domain and application flows (zero business exceptions).
- RFC 7807 Problem Details (`application/problem+json`) for all 4xx and 5xx API error responses.
- Every API endpoint MUST have at least one automated integration test verifying its happy path.
- In-house CQRS implementation (MediatR package forbidden per explicit user directive).

**Scale/Scope**: Foundational workspace (4 backend class libraries, 1 API host project, 2 test projects, and 1 frontend placeholder).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evaluation & Compliance Notes |
|-----------|--------|-------------------------------|
| **I. Domain-Driven Design (DDD), Clean Architecture & Transactional Integrity** | **PASS** | Dependencies point strictly inward. Domain core has zero external framework dependencies. Persistence abstracted behind Repository and Unit of Work (`IUnitOfWork`) pattern. |
| **II. CQRS & Vertical Slice Architecture** | **PASS** | Read and write operations separated into `ICommand<T>` and `IQuery<T>`. Handlers dispatched via clean in-house dispatcher. Features structured as cohesive vertical slices. |
| **III. Test-Driven Development (TDD) & Automated Testing Standards** | **PASS** | xUnit + Moq standardized for unit tests. Turnkey `TaskFyWebApplicationFactory` configured for happy-path integration testing of all API endpoints. |
| **IV. Resilient Validation, Error Handling & RFC 7807 Problem Details** | **PASS** | `Result<T>` and `Error` value objects capture outcomes without exceptions. API middleware translates failures to RFC 7807 Problem Details (`application/problem+json`). |
| **V. Single Responsibility & Strict File Isolation** | **PASS** | Exactly one class, interface, record, or enum per file matching filename in PascalCase. |
| **VI. Frontend Feature-First Architecture & Lazy Loading** | **PASS** | `src/Frontend/` reserved and isolated for Angular 20 implementation in subsequent phase. |
| **VII. Strict Dependency Governance** | **PASS** | All dependencies (EF Core Sqlite, Mapster, Swagger, xUnit, Moq) pre-authorized. MediatR excluded in favor of direct in-house implementation. |

## Project Structure

### Documentation (this feature)

```text
specs/001-backend-init-devex/
├── spec.md                  # Feature specification
├── plan.md                  # This file (/speckit-plan command output)
├── research.md              # Phase 0 output (architectural decisions & trade-offs)
├── data-model.md            # Phase 1 output (core abstractions, Result, CQRS, contracts)
├── quickstart.md            # Phase 1 output (runnable developer validation guide)
├── contracts/               # Phase 1 output (API schema definitions)
│   ├── health-api.json      # Schema for GET /health endpoint
│   └── problem-details.json # RFC 7807 Problem Details schema
├── checklists/
│   └── requirements.md      # Spec quality checklist
└── tasks.md                 # Phase 2 output (/speckit-tasks command)
```

### Source Code (repository root)

```text
TaskFy/
├── .specify/
├── specs/
└── src/
    ├── Backend/
    │   ├── TaskFy.sln
    │   ├── TaskFy.Domain/
    │   │   ├── Common/
    │   │   │   ├── Entity.cs
    │   │   │   ├── ValueObject.cs
    │   │   │   └── IDomainEvent.cs
    │   │   └── Repositories/
    │   │       ├── IRepository.cs
    │   │       └── IUnitOfWork.cs
    │   ├── TaskFy.Application/
    │   │   ├── Common/
    │   │   │   ├── Result.cs
    │   │   │   ├── ResultOfT.cs
    │   │   │   ├── Error.cs
    │   │   │   └── ErrorType.cs
    │   │   ├── Cqrs/
    │   │   │   ├── ICommand.cs
    │   │   │   ├── ICommandOfT.cs
    │   │   │   ├── ICommandHandler.cs
    │   │   │   ├── ICommandHandlerOfT.cs
    │   │   │   ├── IQuery.cs
    │   │   │   ├── IQueryHandler.cs
    │   │   │   ├── IDispatcher.cs
    │   │   │   └── Dispatcher.cs
    │   │   ├── Mappings/
    │   │   │   └── MappingConfig.cs
    │   │   └── DependencyInjection.cs
    │   ├── TaskFy.Infrastructure/
    │   │   ├── Persistence/
    │   │   │   ├── TaskFyDbContext.cs
    │   │   │   └── UnitOfWork.cs
    │   │   └── DependencyInjection.cs
    │   ├── TaskFy.Api/
    │   │   ├── Middleware/
    │   │   │   ├── GlobalExceptionHandlingMiddleware.cs
    │   │   │   └── ProblemDetailsResultExtensions.cs
    │   │   ├── Features/
    │   │   │   └── Diagnostics/
    │   │   │       ├── GetHealthQuery.cs
    │   │   │       ├── GetHealthQueryHandler.cs
    │   │   │       ├── HealthResponseDto.cs
    │   │   │       └── HealthController.cs
    │   │   ├── Program.cs
    │   │   └── appsettings.json
    │   ├── TaskFy.UnitTests/
    │   │   ├── Common/
    │   │   │   ├── ResultTests.cs
    │   │   │   └── ErrorTests.cs
    │   │   └── Cqrs/
    │   │       └── DispatcherTests.cs
    │   └── TaskFy.IntegrationTests/
    │       ├── Fixtures/
    │       │   └── TaskFyWebApplicationFactory.cs
    │       └── Features/
    │           └── Diagnostics/
    │               └── HealthEndpointTests.cs
    └── Frontend/
        └── .gitkeep
```

**Structure Decision**: Clean Architecture combined with Vertical Slices partitioned under `src/Backend/` and `src/Frontend/`. Every source file adheres strictly to the single-type-per-file constraint.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

*No constitutional violations identified. The design strictly adheres to all constitutional principles and user directives.*
