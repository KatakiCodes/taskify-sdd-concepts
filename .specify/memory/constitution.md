# TaskFy Constitution

## Core Principles

### I. Domain-Driven Design (DDD), Clean Architecture & Transactional Integrity
The backend domain core MUST remain completely isolated from frameworks, external libraries, database concerns, and transport protocols. Business logic, state transitions, and business invariants MUST be encapsulated within rich Domain Entities and Aggregate Roots. Domain Events MUST be raised for significant state transitions to decouple side effects and asynchronous workflows. Persistence MUST be abstracted behind the Repository Pattern; repositories return domain aggregates and MUST NOT leak ORM/database-specific query types (e.g., `IQueryable`) outside persistence boundaries.

All data mutation operations MUST be orchestrated through the Unit of Work (`IUnitOfWork`) pattern, enforcing transactional consistency boundaries with explicit commit and automatic rollback semantics upon failure or incomplete operations. Read and query operations MUST be non-tracking by default (`AsNoTracking`); change tracking MUST ONLY be enabled during operations that specifically load entities for state mutation. For read operations involving complex joins, multiple child collections, or heavy datasets (such as tasks with multiple subtasks), split querying (`AsSplitQuery`) MUST be enforced to prevent Cartesian explosion and optimize memory/execution overhead.

Layer dependencies MUST point strictly inward: Domain has zero dependencies; Application depends solely on Domain; Infrastructure and API depend inward on Application and Domain abstractions.

### II. CQRS & Vertical Slice Architecture
Read and write concerns MUST be strictly separated through Command Query Responsibility Segregation (CQRS). Write operations (Commands) handle intent, domain state mutations, and consistency boundaries. Read operations (Queries) project optimized, read-only data transfer objects (DTOs) without triggering side effects. Features SHOULD be organized along Vertical Slices, co-locating command/query definitions, validation handlers, domain service interactions, and endpoint mappings where cohesion is maximized. Slices MUST NOT have hidden coupling or cross-slice domain state mutations.

### III. Test-Driven Development (TDD) Mandatory
Test-Driven Development (TDD) is NON-NEGOTIABLE across both backend and frontend development. The Red-Green-Refactor cycle MUST be strictly enforced: automated tests MUST be written first, verified to fail for the expected reason, satisfied with the minimal necessary implementation, and refactored for clarity and performance. Unit tests MUST validate domain logic, business rules, and state transitions in absolute isolation. Integration tests MUST verify Vertical Slices, repository persistence, and API contract fidelity. Frontend tests MUST cover components, services, pipes, and interceptors with isolated harnesses.

### IV. Resilient Validation & Error Handling (Result/Notification Pattern)
Exceptions MUST NEVER be thrown for predictable business failures, input validation errors, or domain invariant violations. All domain operations, application commands, and queries MUST express operational outcomes using the Result Pattern (`Result<T>` / `Result`) or the Notification Pattern. Errors and invariant violations MUST be captured as strongly-typed error objects or notification collections. ASP.NET Core API endpoints MUST translate Result outcomes to idiomatic HTTP response codes (e.g., 200 OK, 201 Created, 400 Bad Request, 404 Not Found, 422 Unprocessable Entity). Exceptions are strictly reserved for unrecoverable, truly exceptional infrastructural faults (e.g., catastrophic hardware failures, unexpected network disconnection).

### V. Single Responsibility & Strict File Isolation (One Entity/Class per File)
Every class, interface, record, enum, struct, Angular component, service, pipe, directive, or interceptor MUST reside in its own dedicated, individual file. Bundling multiple entities, classes, or models into a single file is STRICTLY FORBIDDEN. High readability for future human code reviews is paramount; code MUST be self-explanatory, cleanly structured, and adhere strictly to SOLID principles. Naming conventions MUST adhere to official industry and framework standards:
- **C# / .NET**: PascalCase for types, methods, properties, and events; camelCase for parameters and local variables; `_camelCase` or standard casing for private fields; filenames MUST exactly match the enclosed type name (e.g., `CreateTaskCommand.cs`, `TaskRepository.cs`).
- **Angular / TypeScript**: kebab-case filenames with descriptive type suffixes (e.g., `task-item.component.ts`, `auth.interceptor.ts`, `task-status.pipe.ts`, `task.model.ts`); PascalCase for class/interface names; camelCase for properties, methods, and functions.

### VI. Frontend Feature-First Architecture & Lazy Loading
The Angular frontend MUST follow a Feature-First directory structure (`features/<feature-name>/...`). Each feature boundary encapsulates its components, state, services, routes, and sub-routing. Route-level Lazy Loading MUST be implemented for all feature modules/routes to ensure minimal initial bundle sizes and fast First Contentful Paint. Frontend design MUST follow official Angular best practices: Standalone components, reactive data paradigms, Angular Pipes for pure transformations, HTTP Interceptors for cross-cutting concerns (authentication headers, error handling, correlation IDs), and clean separation between UI presentation and state management.

### VII. Strict Dependency Governance (Zero Unauthorized Packages)
Installing or introducing third-party external libraries or packages without prior authorization and explicit documentation is STRICTLY PROHIBITED. Every proposed package MUST be documented, vetted, and approved within the feature specifications (`spec.md`), constitution (`constitution.md`), or implementation plan (`plan.md`) before inclusion. Developers MUST prioritize built-in capabilities of the .NET 10 BCL (Base Class Library), ASP.NET Core framework, and Angular 20 standard libraries before considering external dependencies.

## Technology Stack & Architectural Constraints

### Backend (.NET 10 / ASP.NET Core)
- **Runtime & Framework**: .NET 10 with ASP.NET Core Web API.
- **Architecture**: Clean Architecture combined with CQRS and Vertical Slice patterns.
- **Database**: SQLite for relational local storage, managed via structured schema migrations.
- **Persistence & Unit of Work**: Repository Pattern combined with Unit of Work (`IUnitOfWork`). Mutations MUST execute inside transaction boundaries with explicit commit and rollback mechanisms.
- **Query Performance & Tracking Policy**: Non-tracking queries (`AsNoTracking`) MUST be the default for all read operations. Change tracking is strictly restricted to mutation pipelines. Split querying (`AsSplitQuery`) MUST be applied to multi-collection joins (e.g., tasks and subtasks) and heavy queries to eliminate Cartesian explosion.
- **Validation & Results**: Result Pattern / Notification Pattern for all domain, command, and query flows, eliminating unnecessary exceptions.
- **Eventing**: In-process Domain Events dispatched upon aggregate persistence.
- **Testing**: xUnit, NUnit, or standard .NET test runners adhering to TDD; integration tests leveraging `WebApplicationFactory` or in-memory SQLite fixtures.

### Frontend (Angular 20)
- **Framework**: Angular 20 (latest stable release matching environment).
- **Architecture**: Feature-First structure with full lazy loading of feature routes.
- **Best Practices**: Standalone Components, reactive pipelines, strict TypeScript configuration, pure Pipes, HTTP Interceptors, and robust reusable UI components.
- **Styling & Presentation**: Clean, responsive layout adhering to modern UI/UX design standards.
- **Testing**: Automated component and service specs adhering to TDD.

### Code Organization & Quality
- **File Structure**: Exactly one type/class/entity per file across both frontend and backend projects.
- **Readability**: Code MUST be written with clarity as a first-class goal for seamless human review and peer auditing.
- **Clean Contracts**: Strict typing, no untyped `any` in TypeScript, no raw untyped `dynamic`/`object` dictionaries in C# where strong domain types or DTOs can be defined.

## Development Workflow & Quality Gates

### 1. Specification & Approval
Every feature MUST start with an unambiguous specification and implementation plan detailing requirements, vertical slices, entities, and any authorized dependencies.

### 2. TDD Implementation Cycle
1. **Red**: Write a failing unit or contract test verifying the requirement.
2. **Green**: Write the minimum clean code necessary to make the test pass.
3. **Refactor**: Clean up the implementation, ensuring SOLID principles, single-responsibility files, and Result pattern error handling are respected.

### 3. Review Gates & Compliance
Before code can be merged or finalized:
- All unit, integration, and frontend tests MUST pass with 100% success.
- Static analysis, linters, and compiler warnings MUST be addressed.
- Code review MUST verify the One-Type-Per-File constraint and absence of unauthorized dependencies.
- No business exceptions in application or domain layers.
- Queries MUST be audited for non-tracking (`AsNoTracking`) compliance and `AsSplitQuery` adoption on collection joins; transactional mutations MUST employ Unit of Work with rollback handling.

## Governance

This Constitution is the foundational governing document of the TaskFy project and supersedes any conflicting documentation, team habits, or individual coding preferences. Every pull request, task implementation, and code review MUST verify compliance with these principles.

### Amendment Procedure
1. Any modification, addition, or revocation of constitutional principles requires an explicit proposal documenting the rationale, architectural impact, and migration steps.
2. Amendments MUST increment the `CONSTITUTION_VERSION` in compliance with Semantic Versioning:
   - **MAJOR**: Breaking changes to architecture or governance principles (e.g., removing TDD requirement, altering Clean Architecture foundations).
   - **MINOR**: Adding new principles, introducing authorized architectural components, or substantially expanding guidelines.
   - **PATCH**: Wording improvements, non-semantic clarifications, or typo fixes.
3. Upon approval, `.specify/memory/constitution.md` MUST be updated, ratified, and dated accordingly.

**Version**: 1.1.0 | **Ratified**: 2026-10-12 | **Last Amended**: 2026-10-12
