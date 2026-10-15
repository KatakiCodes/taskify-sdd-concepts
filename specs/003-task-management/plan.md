# Implementation Plan: Task Creation & Query Management (003-task-management)

**Branch**: `003-task-management` | **Date**: 2026-10-15 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/003-task-management/spec.md` for task creation, retrieval by ID, and multi-criteria filtered search under an imaginary test user ID.

## Summary

This feature delivers core task lifecycle management using Clean Architecture, DDD, and native CQRS:
1. **Domain Model**: `TaskItem` aggregate root with encapsulation, business invariants (title validation, start/end date consistency), `TaskStatus` enum (`Pending`, `InProgress`, `Completed`, `Cancelled`), and `TaskCreatedDomainEvent`.
2. **Persistence & Migrations**: EF Core SQLite configuration mapping `TaskItem` to `"Tasks"`, with repository implementation `TaskRepository` enforcing non-tracking (`AsNoTracking()`) reads and transactional Unit of Work commits.
3. **CQRS Application Slices**: In-house CQRS handlers for `CreateTaskCommand`, `GetTaskByIdQuery`, and `GetTasksQuery`, returning `Result<T>` and abstracting the current user via `IUserContext`.
4. **API Endpoints**: `TasksController` exposing `POST /tasks`, `GET /tasks/{id}`, and `GET /tasks`, formatting errors via RFC 7807 Problem Details.
5. **Automated Testing Suite**: Full TDD unit test coverage (xUnit + Moq) and mandatory happy-path integration tests for all 3 endpoints (`WebApplicationFactory` with SQLite in-memory).

## Technical Context

**Language/Version**: C# 13 / .NET 10.0
**Primary Dependencies**: EF Core 10 SQLite, Mapster, native CQRS engine (zero external mediator libraries), native ASP.NET Core BCL
**Storage**: SQLite (`taskfy.db`), managed via EF Core schema migrations
**Testing**: xUnit, Moq, `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`)
**Target Platform**: Windows / Linux cross-platform developer workstations & containers
**Project Type**: Web API (`TaskFy.Api`) with Clean Architecture layers
**Performance Goals**: Task creation response < 200ms; filtered queries < 100ms
**Constraints**:
- **C# Disambiguation**: Domain entity named `TaskItem` to avoid collision with `System.Threading.Tasks.Task`.
- **One-Type-Per-File**: Every class, record, interface, or enum in its own file strictly matching its type name.
- **Strict Dependency Governance**: Zero new external NuGet packages.
- **Problem Details**: All 4xx client and 5xx server errors formatted as RFC 7807 payloads.
- **Repository Isolation**: Repositories return domain entities, not `IQueryable`.
**Scale/Scope**: Domain, Application, Infrastructure, API, UnitTests, and IntegrationTests.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate / Principle | Status | Evaluation & Compliance Notes |
| :--- | :---: | :--- |
| **I. Domain-Driven Design & Layer Isolation** | **PASS** | Domain is framework-free. `TaskItem` encapsulates invariants. Persistence abstracted by `ITaskRepository`. Changes committed via `IUnitOfWork`. Non-tracking queries by default. |
| **II. CQRS & Vertical Slice** | **PASS** | Write flows isolated in `Commands/CreateTask`; read projections isolated in `Queries/GetTaskById` and `Queries/GetTasks` returning read-only DTOs. |
| **III. TDD & Automated Testing Standards** | **PASS** | Unit tests with xUnit + Moq; mandatory happy-path integration tests for `POST /tasks`, `GET /tasks/{id}`, and `GET /tasks` using `WebApplicationFactory`. |
| **IV. Resilient Validation & RFC 7807** | **PASS** | Invariants validated via `Result<T>`. Errors mapped to `ValidationProblemDetails` (400) and `ProblemDetails` (404/500). No business exceptions. |
| **V. Single Responsibility & One-Type-Per-File** | **PASS** | Every type resides in its own `.cs` file matching its name. |
| **VII. Strict Dependency Governance** | **PASS** | Zero new external packages. Uses already authorized EF Core SQLite, Mapster, xUnit, Moq. |
| **VIII. Secure Dev Config & Dev CORS** | **PASS** | Endpoints inherit existing development CORS policy and User Secrets architecture. |
| **IX. Git Versioning Governance** | **PASS** | All subsequent implementation commits will follow Conventional Commits in Portuguese, atomic units of work, and imperative mood. |

## Project Structure

### Documentation (this feature)

```text
specs/003-task-management/
├── plan.md              # Implementation plan (this file)
├── research.md          # Technical research & architectural decisions
├── data-model.md        # Entities, relational schema, DTOs, indexes
├── quickstart.md        # Runnable verification guide
├── contracts/
│   └── tasks-api.json   # JSON Schema contract for tasks endpoints
└── tasks.md             # Actionable tasks (generated in next phase by /speckit-tasks)
```

### Source Code (repository layout)

```text
src/
└── Backend/
    ├── TaskFy.Domain/
    │   ├── Entities/
    │   │   └── TaskItem.cs                           # Aggregate root with Create method
    │   ├── Enums/
    │   │   └── TaskStatus.cs                         # Enum: Pending, InProgress, Completed, Cancelled
    │   ├── Events/
    │   │   └── TaskCreatedDomainEvent.cs             # IDomainEvent
    │   ├── Repositories/
    │   │   └── ITaskRepository.cs                    # Returns TaskItem aggregates, zero IQueryable leakage
    │   └── Errors/
    │       └── TaskErrors.cs                         # Strongly typed domain errors (NotFound, InvalidDates, etc.)
    ├── TaskFy.Application/
    │   ├── Common/
    │   │   └── Interfaces/
    │   │       └── IUserContext.cs                   # Current user abstraction
    │   └── Features/
    │       └── Tasks/
    │           ├── DTOs/
    │           │   └── TaskDto.cs                    # Presentation record
    │           ├── Commands/
    │           │   └── CreateTask/
    │           │       ├── CreateTaskCommand.cs      # ICommand<Result<TaskDto>>
    │           │       └── CreateTaskCommandHandler.cs
    │           └── Queries/
    │               ├── GetTaskById/
    │               │   ├── GetTaskByIdQuery.cs       # IQuery<Result<TaskDto>>
    │               │   └── GetTaskByIdQueryHandler.cs
    │               └── GetTasks/
    │                   ├── GetTasksQuery.cs          # IQuery<Result<IReadOnlyList<TaskDto>>>
    │                   └── GetTasksQueryHandler.cs
    ├── TaskFy.Infrastructure/
    │   ├── Persistence/
    │   │   ├── Configurations/
    │   │   │   └── TaskItemConfiguration.cs          # EF Core SQLite table mapping for Tasks
    │   │   ├── Repositories/
    │   │   │   └── TaskRepository.cs                 # Implements ITaskRepository with AsNoTracking()
    │   │   └── Migrations/                           # EF Core schema migration for Tasks
    │   └── DependencyInjection.cs                    # Registers ITaskRepository
    ├── TaskFy.Api/
    │   ├── Common/
    │   │   └── DefaultUserContext.cs                 # Implements IUserContext with mock test GUID
    │   └── Features/
    │       └── Tasks/
    │           ├── TasksController.cs                # ControllerBase endpoints: POST, GET /id, GET
    │           ├── CreateTaskRequest.cs
    │           └── GetTasksRequest.cs
    ├── TaskFy.UnitTests/
    │   ├── Domain/
    │   │   └── TaskItemTests.cs
    │   └── Features/
    │       └── Tasks/
    │           ├── CreateTaskCommandHandlerTests.cs
    │           ├── GetTaskByIdQueryHandlerTests.cs
    │           └── GetTasksQueryHandlerTests.cs
    └── TaskFy.IntegrationTests/
        └── Features/
            └── Tasks/
                ├── CreateTaskEndpointTests.cs
                ├── GetTaskByIdEndpointTests.cs
                └── GetTasksEndpointTests.cs
```

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
| :--- | :--- | :--- |
| *None* | *Fully compliant with Constitution and architectural rules* | *No violations* |
