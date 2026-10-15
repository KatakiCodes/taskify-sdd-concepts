# Research & Architecture Decisions: Backend Initialization & DevEx Baseline

**Feature**: `001-backend-init-devex`  
**Date**: 2026-10-14  
**Status**: Completed

## Executive Summary

This document consolidates architectural research and technology decisions for initializing the TaskFy backend solution. All decisions strictly align with the project constitution (DDD, Clean Architecture, CQRS, TDD, One-Type-Per-File, Result Pattern, RFC 7807 Problem Details) and the user's specific architectural directives (EF Core, SQLite, Swagger, Mapster, and an in-house MediatR-free CQRS pattern).

---

## 1. In-House Lightweight CQRS Abstraction (No MediatR)

### Context & Need
The constitution dictates CQRS with vertical slicing. The user explicitly mandated: *"não usaremos o pacote Mediatr, usaremos uma implementação própria e direta 'hardcoded'"*. We need a clean, decoupled, type-safe command/query execution mechanism with zero third-party mediator dependencies.

### Decision
Implement lightweight, interface-driven CQRS abstractions in `TaskFy.Application`:
- `ICommand<TResult>`: Marker interface for write operations producing `Result<TResult>`.
- `ICommandHandler<TCommand, TResult>`: Handler contract for commands.
- `IQuery<TResult>`: Marker interface for read operations producing `Result<TResult>`.
- `IQueryHandler<TQuery, TResult>`: Handler contract for queries.
- `IDispatcher` (or `ISender`): An in-house dispatcher service backed by `IServiceProvider` that resolves and executes the appropriate handler instance.
- Extension method `services.AddCqrsHandlers(assembly)` to automatically register all handlers with `Scoped` lifetime in the ASP.NET Core DI container.

### Rationale
- **Zero Third-Party Coupling**: Fully complies with the user directive and Principle VII (Strict Dependency Governance).
- **Zero Reflection Overhead at Runtime**: Handlers are resolved directly from DI by their closed generic type `ICommandHandler<TCommand, TResult>`.
- **Testability**: Handlers can be tested in complete isolation by instantiating them directly with mocked dependencies.

### Alternatives Considered
- *MediatR library*: Rejected per user constraint and to avoid unnecessary third-party package dependencies.
- *Raw controller-to-service calls*: Rejected because it promotes bloated services instead of isolated, cohesive vertical slices.

---

## 2. Object Relational Mapping: Entity Framework Core 10 with SQLite

### Context & Need
The application requires local relational storage for development and testing, managed with schema migrations, repository abstraction, and transactional Unit of Work (`IUnitOfWork`).

### Decision
- **Package**: `Microsoft.EntityFrameworkCore.Sqlite` (v10.x matching .NET 10).
- **DbContext**: `TaskFyDbContext` in `TaskFy.Infrastructure`, configured with:
  - Strongly-typed entity configurations via `IEntityTypeConfiguration<T>` in isolated files.
  - Automatic `AsNoTracking` default for queries where applicable.
  - Transaction orchestration implementing `IUnitOfWork` with explicit `CommitAsync()` and rollback semantics.
- **Testing Database Strategy**: Use `Microsoft.Data.Sqlite` with an in-memory connection (`DataSource=:memory:`) kept open for the lifetime of integration test fixtures, ensuring zero disk state, fast execution, and automated schema recreation.

### Rationale
- Complies with Constitution Principle I (DDD, Clean Architecture, Repository + Unit of Work).
- SQLite provides zero-setup relational persistence for local development and rapid, isolated integration test runs.

### Alternatives Considered
- *In-Memory EF Core Provider (`Microsoft.EntityFrameworkCore.InMemory`)*: Rejected because it does not support relational integrity, foreign key constraints, or raw SQL queries, leading to false-positive tests. In-memory SQLite is the official EF Core recommended approach.

---

## 3. Object Mapping: Mapster

### Context & Need
DTO-to-Domain and Domain-to-DTO projection is needed across Application vertical slices, keeping mapping rules declarative and high-performance.

### Decision
- **Package**: `Mapster` and `Mapster.DependencyInjection`.
- **Configuration**:
  - Centralized scan in `TaskFy.Application` using `TypeAdapterConfig.GlobalSettings.Scan(assembly)`.
  - Mappings declared using `IRegister` implementations placed in dedicated files within vertical slice folders.
  - Inject `IMapper` or utilize strongly-typed adapt extensions.

### Rationale
- Exceptionally fast performance (generates IL / direct compiled expressions).
- Fluent, readable configuration matching the One-Type-Per-File constraint.
- Lightweight compared to AutoMapper and requires minimal boilerplate.

---

## 4. API Documentation: Swagger / OpenAPI

### Context & Need
Developers and consumers require interactive API documentation, schema contracts, and endpoint exploration.

### Decision
- **Package**: `Swashbuckle.AspNetCore`.
- **Configuration**:
  - Configure `AddSwaggerGen` with XML doc generation enabled in `TaskFy.Api.csproj`.
  - Register schema definitions for standard contracts, including `ProblemDetails` (`application/problem+json`).
  - Expose Swagger UI in development environment under `/swagger`.

### Rationale
- Industry standard for ASP.NET Core Web APIs.
- Provides interactive test UI and client-consumable OpenAPI v3 schemas.

---

## 5. RFC 7807 Problem Details & Result Pattern Integration

### Context & Need
Constitution Principle IV mandates that business exceptions are forbidden and all API error responses (4xx client/validation and 5xx unhandled faults) must conform strictly to RFC 7807 Problem Details (`application/problem+json`).

### Decision
1. **Application Layer (`Result<T>` & `Error`)**:
   - `Error` value object encapsulating `Code`, `Description`, and `ErrorType` (Validation, NotFound, Conflict, Failure).
   - `Result` and `Result<T>` encapsulating `IsSuccess`, `Value`, and `Error` (or `IReadOnlyList<Error>`).
2. **API Layer Problem Details Factory / Mapping**:
   - An extension method `ToProblemDetails(this Result result)` or an `EndpointResultFilter` that maps:
     - `ErrorType.Validation` → HTTP 400 Bad Request with `ValidationProblemDetails` (containing an `errors` dictionary).
     - `ErrorType.NotFound` → HTTP 404 Not Found `ProblemDetails`.
     - `ErrorType.Conflict` → HTTP 409 Conflict `ProblemDetails`.
     - `ErrorType.Failure` → HTTP 422 Unprocessable Entity or 400 Bad Request `ProblemDetails`.
3. **Global Exception Handling Middleware**:
   - Custom `GlobalExceptionHandlerMiddleware` or `IExceptionHandler` returning an RFC 7807 `ProblemDetails` with HTTP 500 Internal Server Error without leaking sensitive stack traces.

### Rationale
- Completely standardizes error structures across all endpoints.
- Frontends and automated consumers can reliably parse error codes and validation dictionaries.

---

## 6. Testing Harness: xUnit + Moq + WebApplicationFactory

### Context & Need
Constitution Principle III mandates TDD, xUnit + Moq for unit testing, and at least one happy-path integration test per endpoint using `WebApplicationFactory`.

### Decision
1. **`TaskFy.UnitTests`**:
   - `xunit` (v2.x / v3.x) test framework.
   - `moq` for isolating domain services and repository abstractions.
   - Tests verify Result pattern logic, CQRS dispatcher resolution, and business rules in isolation.
2. **`TaskFy.IntegrationTests`**:
   - `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory<Program>`).
   - Custom `TaskFyWebApplicationFactory` that replaces the SQLite connection with an open in-memory SQLite connection fixture.
   - Executes real HTTP requests (`HttpClient`) through the full ASP.NET Core pipeline and verifies status codes, contracts, and Problem Details responses.
   - Reference integration test implemented for the `/health` diagnostic probe.

---

## 7. Folder Structure & Solution Partitioning

### Context & Need
The user specified root workspace partitioning: `src/` containing `Backend/` and `Frontend/`.

### Decision
```text
TaskFy/
├── .specify/
├── specs/
│   └── 001-backend-init-devex/
└── src/
    ├── Backend/
    │   ├── TaskFy.sln
    │   ├── TaskFy.Domain/            # Entities, Enums, Domain Events, Repositories
    │   ├── TaskFy.Application/       # In-house CQRS, Result, Mapster, Slices
    │   ├── TaskFy.Infrastructure/    # EF Core DbContext, SQLite, UoW
    │   ├── TaskFy.Api/               # Endpoints, Swagger, Middleware, Program.cs
    │   ├── TaskFy.UnitTests/         # xUnit + Moq tests
    │   └── TaskFy.IntegrationTests/  # WebApplicationFactory + SQLite tests
    └── Frontend/                     # Reserved for Angular 20
```

All source files adhere strictly to One-Type-Per-File naming conventions.
