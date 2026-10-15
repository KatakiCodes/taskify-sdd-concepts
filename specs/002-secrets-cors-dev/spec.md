# Feature Specification: Development Security & Cross-Origin Experience (User Secrets & Dev CORS)

**Feature Branch**: `002-secrets-cors-dev`

**Created**: 2026-10-15

**Status**: Draft

**Input**: User description: "Adicionar configuração de User Secrets e política de CORS para ambiente de desenvolvimento na API. Para ambiente de desenvolvimento vamos usar a configuração mais genêrica de cors (com anyOrigin, anyMethod e anyHeader)."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Secure Local Configuration Isolation via User Secrets (Priority: P1)

Developers working on local workstations need to configure sensitive credentials, custom connection strings, and local developer tokens without committing sensitive text or keys to source control or shared repository files.

**Why this priority**: Eliminates credential leakage risks and accidental commits of sensitive configuration, adhering directly to Principle VIII of the Project Constitution.

**Independent Test**: A developer configures a sensitive setting in their local machine-isolated developer store. The API boots in development mode, reads the isolated setting, and operates normally while all tracked configuration files (`appsettings.json`, `appsettings.Development.json`) remain completely free of secrets.

**Acceptance Scenarios**:

1. **Given** a developer working in local development mode, **When** a sensitive setting or credential is provided exclusively via the local user secrets store, **Then** the application reads and applies that configuration value without error.
2. **Given** an audit of source-controlled configuration files, **When** inspecting `appsettings.json` and `appsettings.Development.json`, **Then** zero credentials, private keys, or sensitive connection parameters are present.
3. **Given** a developer configuring a setting that also exists in base configuration, **When** the application starts, **Then** the local user secret value cleanly takes precedence over the base configuration value.

---

### User Story 2 - Generic Cross-Origin Access for Local Frontend Development (Priority: P2)

Frontend engineers and external API consumer tools working across different local ports (such as Angular on `http://localhost:4200`, mobile emulators, or local test harnesses) need to make API calls to the local backend without being blocked by browser cross-origin security rejections.

**Why this priority**: Unblocks local full-stack development, enabling frontend and API developers to collaborate with zero friction across local origins.

**Independent Test**: A local client dispatches cross-origin requests (including preflight `OPTIONS` requests) from an arbitrary local origin using any standard HTTP method and custom header. The API responds with headers permitting cross-origin execution and delivers the expected business payload.

**Acceptance Scenarios**:

1. **Given** the API running in development mode, **When** a client sends a cross-origin preflight `OPTIONS` request with custom headers, **Then** the API responds immediately with status 204 or 200 and headers allowing any origin, any method, and any header.
2. **Given** the API running in development mode, **When** a cross-origin `GET` or `POST` request is dispatched from any origin, **Then** the API successfully processes the request and returns the response with the permissive origin access header.
3. **Given** automated integration test execution, **When** testing API diagnostic endpoints (such as `/health`), **Then** requests complete successfully under the active development CORS policy.

---

### Edge Cases

- **Configuration Overlap**: When a configuration key is defined in both `appsettings.Development.json` and the isolated developer store, the developer store value MUST override the file value.
- **Preflight Handling**: When a browser client sends a preflight `OPTIONS` check before issuing a request with custom headers, the server MUST handle the preflight at the middleware level without attempting to execute downstream application controllers or handlers.
- **Environment Isolation**: When the environment is switched away from development, generic cross-origin permissions and local development secrets MUST NOT be assumed or unintentionally exposed.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST support an isolated, machine-specific configuration mechanism for local development that stores sensitive parameters outside the project file directory.
- **FR-002**: The application MUST prioritize machine-isolated developer secrets over repository-stored configuration files when running in the development environment.
- **FR-003**: Repository configuration files (`appsettings.json` and `appsettings.Development.json`) MUST NOT contain passwords, private API keys, or sensitive connection strings.
- **FR-004**: The API MUST enable Cross-Origin Resource Sharing (CORS) when running in the development environment.
- **FR-005**: In the development environment, the CORS policy MUST permit incoming requests from any client origin (`AllowAnyOrigin`).
- **FR-006**: In the development environment, the CORS policy MUST permit incoming requests using any HTTP method (`AllowAnyMethod`).
- **FR-007**: In the development environment, the CORS policy MUST permit incoming requests containing any HTTP header (`AllowAnyHeader`).
- **FR-008**: Existing diagnostic endpoints (such as `/health`) and future feature endpoints MUST remain fully functional and accessible when accessed across origins in development.

### Key Entities

- **DeveloperSecretProfile**: Represents the configuration metadata and unique machine-isolated storage context assigned to the application for local developer credential management.
- **DevelopmentCorsPolicy**: Represents the cross-origin rule set active in development, defining permissions for origins, HTTP verbs, and headers.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of tracked repository configuration files pass automated security and code review audits with zero committed secrets or credentials.
- **SC-002**: Local client applications originating from any domain or port can execute API requests in development without encountering browser CORS rejection errors (0% CORS blocking in development).
- **SC-003**: Preflight cross-origin negotiation overhead remains negligible, resolving in under 50ms for local development calls.
- **SC-004**: 100% of automated unit and integration tests continue to pass cleanly without regression under the development security and CORS configuration.

## Assumptions

- The generic CORS configuration (any origin, any method, any header) is specifically intended for local development convenience and developer agility, as explicitly specified by the project owner.
- Development workstations utilize standard developer tooling supporting local secret isolation.
- Production and staging environments will define restricted, origin-whitelisted CORS policies and dedicated infrastructure secret managers in future operational phases.
