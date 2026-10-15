# Technical Research: Task Creation & Query Management (003-task-management)

## Research Topic 1: Entity Naming in C# (.NET Task Collision Avoidance)

### Decision
Name the domain entity `TaskItem` within `TaskFy.Domain.Entities`, while mapping it to the database table `"Tasks"` and exposing HTTP routes as `/tasks`.

### Rationale
- **Disambiguation**: Naming the class `Task` in C# produces severe namespace ambiguities with the framework-level `System.Threading.Tasks.Task` across all asynchronous methods and interfaces (`Task<Result<...>>`).
- **Clean Developer Experience**: Eliminates the need for awkward `global::System.Threading.Tasks.Task` prefixes throughout handlers, queries, and repositories.
- **REST & Table Alignment**: EF Core allows `builder.ToTable("Tasks")`, preserving idiomatic REST plural `/tasks` and database table naming.

### Alternatives Considered
1. **Naming domain class `Task`**: Evaluated and rejected due to ubiquitous compiler conflicts with `System.Threading.Tasks.Task`.
2. **Naming `TodoTask`**: Rejected as "TaskItem" is more standard and directly aligns with DDD terminology.

---

## Research Topic 2: User Context & Identification (Pre-Authentication Phase)

### Decision
Define an abstraction `IUserContext` in `TaskFy.Application.Common.Interfaces` with property `Guid CurrentUserId { get; }`. Provide a default implementation `DefaultUserContext` in `TaskFy.Api` returning a deterministic development GUID: `00000000-0000-0000-0000-000000000001`.

### Rationale
- Decouples task business logic from the upcoming authentication feature.
- When JWT authentication is added in future phases, `DefaultUserContext` will simply be replaced by an implementation reading `HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)` without touching Domain or Application layers.

### Alternatives Considered
1. **Hardcoding GUID in Command Handlers**: Rejected as it violates Clean Architecture and makes future auth migration difficult.
2. **Passing UserId in API Request Body**: Rejected because client-supplied user identifiers pose security vulnerabilities.

---

## Research Topic 3: Query Filtering & EF Core Performance

### Decision
Implement the search query in `TaskRepository.SearchAsync` using EF Core composable LINQ with:
- Non-tracking (`AsNoTracking()`) enforced by default per Constitution Principle I.
- `EF.Functions.Like` for SQLite case-insensitive partial text search on `Name` and `Description`.
- Conditional `.Where()` clauses for `status`, `createdFrom`, `createdTo`, `dueFrom`, `dueTo`.
- Default ordering by `CreatedAt DESC`.

### Rationale
- Optimizes query execution and memory footprint by avoiding change tracking overhead.
- Leverages SQLite's native text comparison semantics efficiently.

### Alternatives Considered
1. **In-memory filtering after full fetch**: Rejected due to unacceptable memory and latency scaling.
2. **Raw SQL query strings**: Rejected because LINQ with strongly-typed predicates provides compile-time safety and prevents SQL injection.

---

## Research Topic 4: Domain Aggregate Invariants & Error Handling

### Decision
Enforce domain invariants inside a factory method `TaskItem.Create(...)` returning `Result<TaskItem>`:
- `Name`: required, trimmed, 3 to 150 characters.
- `StartDate`: valid DateTimeOffset.
- `EndDate`: optional; if provided, must be `>= StartDate`.
- Initial `Status`: `TaskStatus.Pending`.

### Rationale
- Adheres to DDD aggregate integrity and Constitution Principle IV (no exceptions for predictable validation failures; express outcomes using `Result<T>`).
- API controllers map `Result` failure directly to `ValidationProblemDetails` (RFC 7807).
