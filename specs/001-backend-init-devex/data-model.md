# Data Model & Core Abstractions: Backend Initialization & DevEx Baseline

**Feature**: `001-backend-init-devex`  
**Date**: 2026-10-14  
**Status**: Completed

## 1. Overview

Because Feature `001-backend-init-devex` establishes the foundational architecture and developer experience, its core model encompasses the foundational abstractions that govern all future domain entities, vertical slices, operational outcomes, and API contracts.

---

## 2. Core Abstractions & Value Objects

### 2.1 Result & Error Pattern (`TaskFy.Domain` / `TaskFy.Application`)

Enforces the constitutional non-negotiable rule: **zero business exceptions for flow control**.

#### `ErrorType` (Enum)
Categorizes errors for idiomatic HTTP translation:
- `Failure`: Generic operation failure (Maps to HTTP 400).
- `Validation`: Input or invariant validation failure (Maps to HTTP 400 with `ValidationProblemDetails`).
- `NotFound`: Requested resource was not found (Maps to HTTP 404).
- `Conflict`: Business invariant or concurrency conflict (Maps to HTTP 409).
- `Unexpected`: Unhandled or infrastructural fault (Maps to HTTP 500).

#### `Error` (Value Object)
- **Properties**:
  - `Code`: `string` (Unique machine-readable error identifier, e.g., `"Health.Unavailable"`, `"Task.TitleRequired"`).
  - `Description`: `string` (Human-readable error description).
  - `Type`: `ErrorType` (Category of error).
- **Static Factory Methods**:
  - `Error.Validation(string code, string description)`
  - `Error.NotFound(string code, string description)`
  - `Error.Conflict(string code, string description)`
  - `Error.Failure(string code, string description)`
  - `Error.Unexpected(string code, string description)`

#### `Result` (Non-Generic Value Object)
- **Properties**:
  - `IsSuccess`: `bool`
  - `IsFailure`: `bool` (`!IsSuccess`)
  - `Error`: `Error`
- **Factory Methods**:
  - `Result.Success()`
  - `Result.Failure(Error error)`

#### `Result<T>` (Generic Value Object)
- **Properties**:
  - `IsSuccess`: `bool`
  - `IsFailure`: `bool`
  - `Value`: `T` (Throws `InvalidOperationException` if accessed when `IsFailure` is true).
  - `Error`: `Error`
- **Factory Methods**:
  - `Result<T>.Success(T value)`
  - `Result<T>.Failure(Error error)`
  - Implicit conversion operators from `T` to `Result<T>` and `Error` to `Result<T>`.

---

## 3. In-House CQRS Abstractions (`TaskFy.Application`)

Lightweight, in-house messaging contracts without MediatR.

#### `ICommand<TResult>` & `ICommand` (Interfaces)
- Marker interface for write operations intended to mutate state.
- `ICommand<TResult>` produces `Result<TResult>`.
- `ICommand` produces `Result`.

#### `ICommandHandler<TCommand, TResult>` (Interface)
- **Method**:
  ```csharp
  Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
  ```

#### `IQuery<TResult>` (Interface)
- Marker interface for read operations returning projected data without side effects.
- Produces `Result<TResult>`.

#### `IQueryHandler<TQuery, TResult>` (Interface)
- **Method**:
  ```csharp
  Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
  ```

#### `IDispatcher` (Interface & Service)
- Direct dispatcher resolving handlers via ASP.NET Core `IServiceProvider`.
- **Methods**:
  ```csharp
  Task<Result<TResult>> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
  Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);
  Task<Result<TResult>> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
  ```

---

## 4. Transactional Persistence & Unit of Work (`TaskFy.Domain` & `TaskFy.Infrastructure`)

#### `IUnitOfWork` (Interface in `TaskFy.Domain`)
Enforces atomic transactional consistency boundaries:
```csharp
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
```

#### `TaskFyDbContext` (Implementation in `TaskFy.Infrastructure`)
- Extends EF Core `DbContext`.
- Implements `IUnitOfWork`.
- Configured for SQLite database engine (`Microsoft.EntityFrameworkCore.Sqlite`).
- Explicit configuration classes implementing `IEntityTypeConfiguration<TEntity>` stored in individual files.

---

## 5. API Contracts & Diagnostic Entities (`TaskFy.Api`)

### 5.1 Health Check Diagnostic (`HealthCheckResponse`)
- **Fields**:
  - `Status`: `string` (e.g., `"Healthy"`, `"Degraded"`, `"Unhealthy"`).
  - `Timestamp`: `DateTimeOffset` (ISO-8601 UTC timestamp).
  - `Version`: `string` (Application/API version, e.g., `"1.0.0"`).
  - `Environment`: `string` (e.g., `"Development"`, `"Production"`).
  - `Duration`: `TimeSpan` (Probe execution duration).

### 5.2 RFC 7807 Problem Details Schema
- **Fields**:
  - `type`: `string` (URI reference identifying the problem type, e.g., `"https://tools.ietf.org/html/rfc7231#section-6.5.1"`).
  - `title`: `string` (Short summary of the problem type).
  - `status`: `int` (HTTP status code).
  - `detail`: `string` (Human-readable explanation specific to this occurrence).
  - `instance`: `string` (URI reference identifying the specific endpoint/request path).
  - `extensions`: `Dictionary<string, object>` (Contains custom metadata such as `code` and an `errors` collection for validation failures).
