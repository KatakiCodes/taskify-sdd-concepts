# Quickstart Validation Guide: Backend Initialization & DevEx Baseline

**Feature**: `001-backend-init-devex`  
**Date**: 2026-10-14  
**Status**: Ready for Validation

## 1. Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/) installed (`dotnet --version` outputs `10.x.x`).
- Git installed.
- PowerShell or standard terminal.

---

## 2. Workspace & Architecture Layout

After checking out the branch `001-backend-init-devex`, the repository root contains:
```text
TaskFy/
├── src/
│   ├── Backend/
│   │   ├── TaskFy.sln
│   │   ├── TaskFy.Domain/
│   │   ├── TaskFy.Application/
│   │   ├── TaskFy.Infrastructure/
│   │   ├── TaskFy.Api/
│   │   ├── TaskFy.UnitTests/
│   │   └── TaskFy.IntegrationTests/
│   └── Frontend/               # Reserved for Angular 20
```

---

## 3. Build & Verification Commands

### Step 3.1: Build Solution
Run from the repository root:
```bash
dotnet build src/Backend/TaskFy.sln
```
**Expected Outcome**: Build succeeds with 0 errors and 0 warnings.

### Step 3.2: Run All Tests (Unit & Integration)
Run from the repository root:
```bash
dotnet test src/Backend/TaskFy.sln --logger "console;verbosity=normal"
```
**Expected Outcome**:
- `TaskFy.UnitTests`: 100% passed (tests validating `Result<T>`, `Error`, CQRS dispatcher resolution, and isolation).
- `TaskFy.IntegrationTests`: 100% passed (verifies `TaskFyWebApplicationFactory`, in-memory SQLite fixture, and `/health` happy-path endpoint response).

---

## 4. Running the API Locally & Verifying Swagger

### Step 4.1: Launch the API
Run from the repository root:
```bash
dotnet run --project src/Backend/TaskFy.Api
```
The console will display output indicating the server is listening, e.g.:
`Now listening on: https://localhost:5001 (or http://localhost:5000)`.

### Step 4.2: Verify Swagger Documentation
Open your browser at:
`https://localhost:5001/swagger`
**Expected Outcome**: Swagger UI loads, displaying the `/health` endpoint and the `ProblemDetails` schema definitions.

---

## 5. End-to-End Validation Scenarios

### Scenario A: Health Probe Happy Path
Send an HTTP GET request to `/health`:
```bash
curl -i -k https://localhost:5001/health
```
**Expected Outcome**:
- Status: `HTTP/1.1 200 OK`
- Header: `Content-Type: application/json`
- Response Payload conforms to [contracts/health-api.json](contracts/health-api.json):
  ```json
  {
    "status": "Healthy",
    "timestamp": "2026-10-14T12:00:00Z",
    "version": "1.0.0",
    "environment": "Development"
  }
  ```

### Scenario B: RFC 7807 Problem Details Error Handling
Send an invalid HTTP request to verify RFC 7807 error formatting:
```bash
curl -i -k https://localhost:5001/health/invalid-sub-route
```
**Expected Outcome**:
- Status: `HTTP/1.1 404 Not Found`
- Header: `Content-Type: application/problem+json`
- Response Payload conforms to [contracts/problem-details.json](contracts/problem-details.json):
  ```json
  {
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
    "title": "Not Found",
    "status": 404,
    "detail": "The requested resource was not found.",
    "instance": "/health/invalid-sub-route"
  }
  ```

### Scenario C: File Isolation Compliance Check
Verify that all source code files adhere to Constitution Principle V (One Type Per File):
- In C#, each file declares strictly one type matching the filename.
- No bundled multi-entity files exist in `TaskFy.Domain` or `TaskFy.Application`.
