<!--
SYNC IMPACT REPORT
- Version Change: 1.4.0 -> 1.5.0
- Bump Rationale: Minor bump - Added Principle IX for Atomic, Semantic & Portuguese Git Versioning Governance, added Section 4 on Git Versioning Flow & Branching Standard, and updated Review Gates.
- Modified Principles: None renamed.
- Added Principles/Sections:
  - Principle IX: Atomic, Semantic & Portuguese Git Versioning Governance
  - Development Workflow & Quality Gates: Section 4 on Git Versioning Flow & Branching Standard
  - Review Gates: Verification of Conventional Commits in Portuguese and atomic staging
- Removed Sections: None.
- Follow-up TODOs: None.
-->

# TaskFy Constitution

## Core Principles

### I. Domain-Driven Design (DDD), Clean Architecture & Transactional Integrity
The backend domain core MUST remain completely isolated from frameworks, external libraries, database concerns, and transport protocols. Business logic, state transitions, and business invariants MUST be encapsulated within rich Domain Entities and Aggregate Roots. Domain Events MUST be raised for significant state transitions to decouple side effects and asynchronous workflows. Persistence MUST be abstracted behind the Repository Pattern; repositories return domain aggregates and MUST NOT leak ORM/database-specific query types (e.g., `IQueryable`) outside persistence boundaries.

All data mutation operations MUST be orchestrated through the Unit of Work (`IUnitOfWork`) pattern, enforcing transactional consistency boundaries with explicit commit and automatic rollback semantics upon failure or incomplete operations. Read and query operations MUST be non-tracking by default (`AsNoTracking`); change tracking MUST ONLY be enabled during operations that specifically load entities for state mutation. For read operations involving complex joins, multiple child collections, or heavy datasets (such as tasks with multiple subtasks), split querying (`AsSplitQuery`) MUST be enforced to prevent Cartesian explosion and optimize memory/execution overhead.

Layer dependencies MUST point strictly inward: Domain has zero dependencies; Application depends solely on Domain; Infrastructure and API depend inward on Application and Domain abstractions.

### II. CQRS & Vertical Slice Architecture
Read and write concerns MUST be strictly separated through Command Query Responsibility Segregation (CQRS). Write operations (Commands) handle intent, domain state mutations, and consistency boundaries. Read operations (Queries) project optimized, read-only data transfer objects (DTOs) without triggering side effects. Features SHOULD be organized along Vertical Slices, co-locating command/query definitions, validation handlers, domain service interactions, and endpoint mappings where cohesion is maximized. Slices MUST NOT have hidden coupling or cross-slice domain state mutations.

### III. Test-Driven Development (TDD) & Automated Testing Standards
Test-Driven Development (TDD) is NON-NEGOTIABLE across both backend and frontend development. The Red-Green-Refactor cycle MUST be strictly enforced: automated tests MUST be written first, verified to fail for the expected reason, satisfied with the minimal necessary implementation, and refactored for clarity and performance.

1. **Unit Testing Framework & Mocking**: Backend unit tests MUST be implemented using **xUnit** as the testing framework combined with **Moq** for test isolation, mock generation, and behavior verification. Unit tests MUST validate domain logic, business rules, and state transitions in absolute isolation from infrastructural components.
2. **Endpoint Integration Testing (Happy Path Mandatory)**: Every backend API endpoint MUST have at least one automated integration test covering its happy path (verifying successful end-to-end execution, HTTP status code, expected response contract, and proper database state persistence). Moq MUST be used in integration test suites to substitute external services, third-party gateways, or unmanaged external dependencies while testing the end-to-end ASP.NET Core pipeline.
3. **Frontend Testing**: Frontend tests MUST cover components, services, pipes, and interceptors using isolated test harnesses and reactive testing patterns.

### IV. Resilient Validation, Error Handling & RFC 7807 Problem Details
Exceptions MUST NEVER be thrown for predictable business failures, input validation errors, or domain invariant violations. All domain operations, application commands, and queries MUST express operational outcomes using the Result Pattern (`Result<T>` / `Result`) or the Notification Pattern. Errors and invariant violations MUST be captured as strongly-typed error objects or notification collections.

ASP.NET Core API endpoints MUST translate failure Result outcomes and unhandled exceptions into standardized **RFC 7807 Problem Details** payloads (`application/problem+json`). Validation failures MUST use standard `ValidationProblemDetails` (or `ProblemDetails` with an `errors` collection), mapping domain error codes, titles, HTTP status codes, and actionable detail messages without leaking raw stack traces or internal implementation secrets. Successful outcomes translate to idiomatic HTTP response codes (e.g., 200 OK, 201 Created). Exceptions are strictly reserved for unrecoverable, truly exceptional infrastructural faults (e.g., catastrophic hardware failures, unexpected network disconnection) and MUST be captured by global exception handling middleware that formats the response as an RFC 7807 Problem Details object with HTTP 500 Internal Server Error.

### V. Single Responsibility & Strict File Isolation (One Entity/Class per File)
Every class, interface, record, enum, struct, Angular component, service, pipe, directive, or interceptor MUST reside in its own dedicated, individual file. Bundling multiple entities, classes, or models into a single file is STRICTLY FORBIDDEN. High readability for future human code reviews is paramount; code MUST be self-explanatory, cleanly structured, and adhere strictly to SOLID principles. Naming conventions MUST adhere to official industry and framework standards:
- **C# / .NET**: PascalCase for types, methods, properties, and events; camelCase for parameters and local variables; `_camelCase` or standard casing for private fields; filenames MUST exactly match the enclosed type name (e.g., `CreateTaskCommand.cs`, `TaskRepository.cs`).
- **Angular / TypeScript**: kebab-case filenames with descriptive type suffixes (e.g., `task-item.component.ts`, `auth.interceptor.ts`, `task-status.pipe.ts`, `task.model.ts`); PascalCase for class/interface names; camelCase for properties, methods, and functions.

### VI. Frontend Feature-First Architecture & Lazy Loading
The Angular frontend MUST follow a Feature-First directory structure (`features/<feature-name>/...`). Each feature boundary encapsulates its components, state, services, routes, and sub-routing. Route-level Lazy Loading MUST be implemented for all feature modules/routes to ensure minimal initial bundle sizes and fast First Contentful Paint. Frontend design MUST follow official Angular best practices: Standalone components, reactive data paradigms, Angular Pipes for pure transformations, HTTP Interceptors for cross-cutting concerns (authentication headers, error handling, correlation IDs), and clean separation between UI presentation and state management.

### VII. Strict Dependency Governance (Zero Unauthorized Packages)
Installing or introducing third-party external libraries or packages without prior authorization and explicit documentation is STRICTLY PROHIBITED. Every proposed package MUST be documented, vetted, and approved within the feature specifications (`spec.md`), constitution (`constitution.md`), or implementation plan (`plan.md`) before inclusion. Developers MUST prioritize built-in capabilities of the .NET 10 BCL (Base Class Library), ASP.NET Core framework, and Angular 20 standard libraries before considering external dependencies. Pre-authorized testing libraries for .NET are explicitly designated as `xunit` and `Moq`.

### VIII. Secure Development Configuration & Cross-Origin Governance (User Secrets & Dev CORS)
Sensitive configuration values—including database credentials, API keys, third-party secrets, and private tokens—MUST NEVER be committed to source control or hardcoded in `appsettings.json` or `appsettings.Development.json`. In the `Development` environment, the backend MUST utilize **.NET User Secrets** (`dotnet user-secrets`) to store and access sensitive keys locally outside the project directory.

Cross-Origin Resource Sharing (CORS) MUST be explicitly configured for the `Development` environment to permit communication with authorized local frontend origins (such as `http://localhost:4200` for Angular development), enabling necessary HTTP methods and headers while strictly prohibiting unauthenticated wildcard open access (`AllowAnyOrigin`) with credentials in production-bound configurations.

### IX. Atomic, Semantic & Portuguese Git Versioning Governance
All version control operations across the repository MUST adhere to disciplined, predictable, and traceable Git practices:
1. **Semantic Commits (Conventional Commits)**: Commits MUST follow the Conventional Commits specification format: `<tipo>(<escopo opcional>): <descrição no imperativo>`. The allowed types are:
   - `feat`: Nova funcionalidade para o usuário, API ou desenvolvedor.
   - `fix`: Correção de bug ou comportamento incorreto.
   - `docs`: Alterações exclusivamente em documentação, especificações (`specs/`), planos ou markdown.
   - `style`: Formatação, espaçamento ou ajustes cosméticos que não alteram a lógica do código.
   - `refactor`: Refatoração de código que não adiciona funcionalidade nem corrige bug.
   - `test`: Criação ou ajuste de testes automatizados (unitários, integração ou arquitetura).
   - `chore`: Atualização de tarefas de build, arquivos de configuração de ferramentas, scripts ou dependências.
   - `perf`: Alterações de código que melhoram mensuravelmente a performance.
2. **Commit Messages Strictly in Portuguese**: All commit messages (summary and body) MUST be written in **Português**. The subject line MUST use the imperative mood (e.g., `adicionar`, `corrigir`, `implementar`, `remover` instead of past or gerund forms like `adicionado` or `adicionando`), start with a lowercase letter after the colon, omit a trailing period, and MUST NOT exceed 72 characters.
3. **Atomic Commits**: Each commit MUST represent a single logical, cohesive, and complete unit of work. Mixing unrelated concerns (e.g., refactoring an existing repository while adding a new feature in the same commit) is STRICTLY PROHIBITED. Every individual commit MUST leave the repository in a stable state that compiles cleanly and passes existing automated tests.
4. **Repository Hygiene & Clean Git History**:
   - Generic, meaningless, or lazy commit messages (such as `wip`, `ajustes`, `fix`, `update`, `checkpoint`, `commiting changes`) are STRICTLY FORBIDDEN.
   - Generated files, build outputs (`bin/`, `obj/`, `out/`), SQLite databases (`*.db`, `*.db-shm`, `*.db-wal`), local IDE settings (`.vs/`, `.vscode/`), and local secrets MUST NEVER be staged or committed.
   - When introducing breaking changes, the commit message MUST include `BREAKING CHANGE:` in the body or footer, explicitly detailing the migration impact.

## Technology Stack & Architectural Constraints

### Backend (.NET 10 / ASP.NET Core)
- **Runtime & Framework**: .NET 10 with ASP.NET Core Web API.
- **Architecture**: Clean Architecture combined with CQRS and Vertical Slice patterns.
- **Database**: SQLite for relational local storage, managed via structured schema migrations.
- **Persistence & Unit of Work**: Repository Pattern combined with Unit of Work (`IUnitOfWork`). Mutations MUST execute inside transaction boundaries with explicit commit and rollback mechanisms.
- **Query Performance & Tracking Policy**: Non-tracking queries (`AsNoTracking`) MUST be the default for all read operations. Change tracking is strictly restricted to mutation pipelines. Split querying (`AsSplitQuery`) MUST be applied to multi-collection joins (e.g., tasks and subtasks) and heavy queries to eliminate Cartesian explosion.
- **Validation, Results & Error Representation**: Result Pattern / Notification Pattern for all domain, command, and query flows, eliminating unnecessary exceptions. All API error responses (4xx client/validation errors and 5xx server faults) MUST strictly adhere to the **RFC 7807 Problem Details** standard (`application/problem+json`).
- **Configuration & Secrets Management**: Sensitive data MUST NEVER reside in `appsettings.json` or `appsettings.Development.json`. In `Development`, **.NET User Secrets** (`dotnet user-secrets`) MUST be utilized to manage secrets locally. Non-sensitive fallback settings are declared in `appsettings.json`.
- **CORS Policy**: An explicit development CORS policy MUST be configured in ASP.NET Core to support local frontend development (e.g., Angular on `http://localhost:4200`), restricting origins and avoiding insecure wildcard policies.
- **Eventing**: In-process Domain Events dispatched upon aggregate persistence.
- **Testing Stack**: **xUnit** and **Moq** for unit testing; integration tests leveraging `WebApplicationFactory` and in-memory SQLite fixtures (using Moq for external/third-party boundaries). Every API endpoint MUST have at least one happy path integration test.

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

### 2. TDD & Endpoint Implementation Flow
1. **Red**: Write a failing unit or contract test verifying the requirement. For any endpoint, define its integration test contract upfront or alongside the endpoint slice.
2. **Green**: Write the minimum clean code necessary to make the test pass.
3. **Refactor**: Clean up the implementation, ensuring SOLID principles, single-responsibility files, and Result pattern error handling are respected.
4. **Endpoint Integration Gate**: Every API endpoint MUST be accompanied by at least one passing integration test covering its happy path (verifying HTTP status code, request validation, payload response, and database state). An endpoint is not considered done without this test.

### 3. Review Gates & Compliance
Before code can be merged or finalized:
- All unit, integration, and frontend tests MUST pass with 100% success.
- Unit tests MUST use **xUnit** and **Moq**.
- Every API endpoint MUST have at least one verified happy-path integration test.
- API error responses conform strictly to the **RFC 7807 Problem Details** specification for both business validation failures and infrastructural faults.
- Configuration files (`appsettings*.json`) MUST NOT contain secrets, credentials, or sensitive connection parameters; local development secrets MUST use User Secrets.
- Development CORS policy is explicitly declared and bound to authorized frontend origins.
- Commit history adheres strictly to **Conventional Commits em Português** with atomic staging and imperative tone.
- Static analysis, linters, and compiler warnings MUST be addressed.
- Code review MUST verify the One-Type-Per-File constraint and absence of unauthorized dependencies.
- No business exceptions in application or domain layers.
- Queries MUST be audited for non-tracking (`AsNoTracking`) compliance and `AsSplitQuery` adoption on collection joins; transactional mutations MUST employ Unit of Work with rollback handling.

### 4. Git Versioning Flow & Branching Standard
1. **Branch Naming**: Feature branches MUST follow the Spec Kit convention: `<prefix>-<short-name>` (e.g., `001-backend-init-devex`, `002-secrets-cors-dev`).
2. **Progressive Atomic Commits**: Commits MUST be made progressively as each logical task or test passes, avoiding single monolithic commits at the end of a sprint or feature.
3. **Commit Anatomy**:
   ```text
   <tipo>(<escopo>): <descrição curta no imperativo>

   [Corpo opcional explicando o contexto, motivação ou decisão técnica]
   ```
   *Exemplos Válidos:*
   - `feat(api): configurar política de cors para ambiente de desenvolvimento`
   - `test(api): adicionar testes de integração para preflight options`
   - `docs(constitution): adicionar regras de versionamento git e commits semânticos`
   - `chore(api): inicializar user secrets id no csproj`

## Governance

This Constitution is the foundational governing document of the TaskFy project and supersedes any conflicting documentation, team habits, or individual coding preferences. Every pull request, task implementation, and code review MUST verify compliance with these principles.

### Amendment Procedure
1. Any modification, addition, or revocation of constitutional principles requires an explicit proposal documenting the rationale, architectural impact, and migration steps.
2. Amendments MUST increment the `CONSTITUTION_VERSION` in compliance with Semantic Versioning:
   - **MAJOR**: Breaking changes to architecture or governance principles (e.g., removing TDD requirement, altering Clean Architecture foundations).
   - **MINOR**: Adding new principles, introducing authorized architectural components, or substantially expanding guidelines.
   - **PATCH**: Wording improvements, non-semantic clarifications, or typo fixes.
3. Upon approval, `.specify/memory/constitution.md` MUST be updated, ratified, and dated accordingly.

**Version**: 1.5.0 | **Ratified**: 2026-10-12 | **Last Amended**: 2026-10-15
