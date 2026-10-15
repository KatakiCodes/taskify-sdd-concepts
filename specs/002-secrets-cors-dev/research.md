# Technical Research: Development Security & Cross-Origin Experience (User Secrets & Dev CORS)

## Research Topic 1: Modular CORS Configuration (Separation from `Program.cs`)

### Decision
Encapsulate CORS service registration and middleware activation inside a dedicated extension class: `src/Backend/TaskFy.Api/Extensions/CorsExtensions.cs` exposing two extension methods:
- `public static IServiceCollection AddDevelopmentCors(this IServiceCollection services, string policyName = CorsExtensions.DefaultDevelopmentPolicyName)`
- `public static IApplicationBuilder UseDevelopmentCors(this IApplicationBuilder app, string policyName = CorsExtensions.DefaultDevelopmentPolicyName)`

### Rationale
- **Decoupled Architecture**: Directly fulfills the user requirement that CORS configuration must not be embedded directly inside `Program.cs`.
- **Single Responsibility**: `Program.cs` remains a concise, high-level orchestration entrypoint (`builder.Services.AddDevelopmentCors()` and `app.UseDevelopmentCors()`).
- **Constitutional Compliance**: The file defines a single static class (`CorsExtensions`), adhering strictly to Principle V (*One-Type-Per-File*).

### Alternatives Considered
1. **Direct inline lambda inside `Program.cs`**: Rejected per explicit user instruction.
2. **Placing inside an omnibus `DependencyInjection.cs`**: Rejected because separating middleware invocation (`UseDevelopmentCors`) from service collection configuration (`AddDevelopmentCors`) is cleaner and more explicit when contained in a focused feature extension file.

---

## Research Topic 2: Generic Development CORS Policy Details & Constraints

### Decision
Configure the development CORS policy (`DevelopmentCorsPolicy`) using ASP.NET Core native methods:
```csharp
policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
```

### Rationale
- Matches the user's explicit specification: *"Para ambiente de desenvolvimento vamos usar a configuração mais genêrica de cors (com anyOrigin, anyMethod e anyHeader)"*.
- Provides zero friction for frontend engineers running Angular (`http://localhost:4200`), Vite, Swagger UI, or API diagnostic tools across varying ports.

### Critical Constraint (ASP.NET Core Runtime Safety)
- In ASP.NET Core, calling `.AllowCredentials()` together with `.AllowAnyOrigin()` throws an `InvalidOperationException` at startup because the CORS specification prohibits wildcard origins with credentials (`Access-Control-Allow-Credentials: true` requires an explicit, non-wildcard origin). Therefore, `.AllowCredentials()` MUST NOT be paired with `.AllowAnyOrigin()`.

### Alternatives Considered
1. **Explicit origin whitelist (`http://localhost:4200`)**: Evaluated for production readiness, but rejected for this development-focused slice per user instruction requiring generic origin access.

---

## Research Topic 3: .NET 10 User Secrets Provisioning

### Decision
Provision a unique `<UserSecretsId>` element in `src/Backend/TaskFy.Api/TaskFy.Api.csproj`.

### Rationale
- Supported natively by the .NET 10 Base Class Library and ASP.NET Core WebApplication host (`WebApplication.CreateBuilder(args)` automatically loads User Secrets in `Development` mode).
- Requires zero external third-party dependencies, adhering strictly to Constitution Principle VII (*Zero Unauthorized Packages*).
- Stores sensitive development values in the user profile directory (`%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json` on Windows / `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json` on Linux/macOS), ensuring that keys never leak into the git repository.

### Alternatives Considered
1. **`.env` files**: Requires third-party packages (e.g. `dotenv.net`) which violates Principle VII.
2. **Machine Environment Variables**: Useful in CI/CD, but cumbersome for developers switching between local repositories compared to standard `dotnet user-secrets set Key Value`.

---

## Research Topic 4: Automated Testing & Validation Pattern

### Decision
Implement integration tests in `TaskFy.IntegrationTests/Features/Cors/CorsEndpointTests.cs` using `TaskFyWebApplicationFactory`:
- Dispatch an HTTP request with header `Origin: http://localhost:4200` to `GET /health` and assert response header `Access-Control-Allow-Origin: *`.
- Dispatch a preflight `OPTIONS` request with headers `Origin: http://localhost:4200` and `Access-Control-Request-Method: GET` and assert HTTP 200 or 204 with `Access-Control-Allow-Methods` and `Access-Control-Allow-Origin`.
- Verify unit tests for `CorsExtensions` registration.

### Rationale
- Enforces the constitutional requirement of automated integration testing for API behavior.
- Verifies that cross-origin negotiation does not interfere with the existing `/health` endpoint or problem details error formatting.
