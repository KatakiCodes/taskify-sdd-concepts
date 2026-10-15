# Implementation Plan: Development Security & Cross-Origin Experience (User Secrets & Dev CORS)

**Branch**: `002-secrets-cors-dev` | **Date**: 2026-10-15 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-secrets-cors-dev/spec.md` with explicit architectural constraint: *"Garanta que a configuração do cors não esteja a ser implementada diretamente no Program.cs"*.

## Summary

This feature establishes secure local secret isolation and friction-free cross-origin communication for local frontend and API developers:
1. **User Secrets Provisioning**: Configures `<UserSecretsId>` in `TaskFy.Api.csproj`, enabling .NET User Secrets to store developer credentials and tokens outside source control while maintaining zero secrets in `appsettings*.json`.
2. **Modular Development CORS**: Configures generic Cross-Origin Resource Sharing (`AllowAnyOrigin`, `AllowAnyMethod`, `AllowAnyHeader`) exclusively active in `Development` mode. **To avoid cluttering `Program.cs`, all CORS policy logic and service setups are cleanly encapsulated inside a dedicated extension class: `CorsExtensions.cs`**. `Program.cs` solely acts as an orchestrator invoking the extension methods.
3. **Automated Integration Testing**: Verifies CORS headers and preflight handling end-to-end using `TaskFyWebApplicationFactory` in `TaskFy.IntegrationTests`.

## Technical Context

**Language/Version**: C# 13 / .NET 10.0
**Primary Dependencies**: ASP.NET Core (`Microsoft.AspNetCore.App` shared framework - native CORS and User Secrets capabilities; zero third-party packages)
**Storage**: N/A (configuration and runtime pipeline only)
**Testing**: xUnit, Moq, `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`)
**Target Platform**: Windows / Linux cross-platform developer workstations
**Project Type**: Web API (`TaskFy.Api`)
**Performance Goals**: Cross-origin preflight `OPTIONS` response overhead < 50ms in local environment
**Constraints**:
- **Separation of Concerns**: CORS configuration MUST NOT be defined inline in `Program.cs`; it MUST be encapsulated in `TaskFy.Api/Extensions/CorsExtensions.cs`.
- **One-Type-Per-File**: Every class/record/interface must reside in its own file matching the type name.
- **Strict Dependency Governance**: Zero new external NuGet packages (native ASP.NET Core BCL only).
- **Environment Isolation**: Permissive CORS policy is active ONLY when `Environment.IsDevelopment()` is true.
**Scale/Scope**: API layer (`TaskFy.Api`) and Integration Test layer (`TaskFy.IntegrationTests`).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Gate / Principle | Status | Evaluation & Compliance Notes |
| :--- | :---: | :--- |
| **I. Domain-Driven Design & Layer Isolation** | **PASS** | Domain and Application layers remain completely untouched. Changes are isolated to API infrastructure and configuration. |
| **II. CQRS & Vertical Slice** | **PASS** | No business commands or queries are introduced; cross-cutting configuration adheres to API slice organization. |
| **III. TDD & Automated Testing Standards** | **PASS** | Integration tests with xUnit and `WebApplicationFactory` will validate CORS preflight and actual requests end-to-end. |
| **IV. RFC 7807 Problem Details** | **PASS** | CORS middleware integrates cleanly upstream; error handling and problem details responses are completely preserved. |
| **V. Single Responsibility & One-Type-Per-File** | **PASS** | `CorsExtensions.cs` and `CorsEndpointTests.cs` each contain exactly one top-level type matching the filename. |
| **VII. Strict Dependency Governance** | **PASS** | 0 new packages; uses built-in ASP.NET Core framework abstractions. |
| **VIII. Secure Dev Config & CORS Governance** | **PASS** | Fulfills User Secrets isolation outside git and explicit development CORS policy. |

## Project Structure

### Documentation (this feature)

```text
specs/002-secrets-cors-dev/
├── plan.md              # Implementation plan (this file)
├── research.md          # Technical research & architectural decisions
├── data-model.md        # Configuration descriptors & precedence hierarchy
├── quickstart.md        # Runnable verification guide
├── contracts/
│   └── cors-api.json    # JSON Schema specification for CORS requests & responses
└── tasks.md             # Actionable tasks (generated in next phase by /speckit-tasks)
```

### Source Code (repository layout)

```text
src/
└── Backend/
    ├── TaskFy.Api/
    │   ├── TaskFy.Api.csproj                         # Provision <UserSecretsId>
    │   ├── Extensions/
    │   │   └── CorsExtensions.cs                     # Encapsulated AddDevelopmentCors & UseDevelopmentCors
    │   └── Program.cs                                # Calls builder.Services.AddDevelopmentCors() & app.UseDevelopmentCors()
    └── TaskFy.IntegrationTests/
        └── Features/
            └── Cors/
                └── CorsEndpointTests.cs              # xUnit integration tests for preflight OPTIONS & cross-origin GET
```

**Structure Decision**:
- All CORS service configuration and middleware logic reside in `src/Backend/TaskFy.Api/Extensions/CorsExtensions.cs`.
- `Program.cs` stays clean and delegates CORS initialization through expressive extension methods, strictly fulfilling the user requirement.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
| :--- | :--- | :--- |
| *None* | *Fully compliant with Constitution and architectural rules* | *No violations* |
