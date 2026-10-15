# Quickstart & Verification Guide: Development Security & Cross-Origin Experience

## Overview
This guide provides runnable instructions to verify local User Secrets provisioning and development Cross-Origin Resource Sharing (CORS) behavior on the TaskFy API.

---

## Scenario 1: Initializing & Managing User Secrets

### 1.1 Verify Secrets Provider Configuration
Ensure the API project specifies a `<UserSecretsId>` in `src/Backend/TaskFy.Api/TaskFy.Api.csproj`.

### 1.2 Store a Local Development Secret
From the project directory, store an isolated test credential:
```powershell
dotnet user-secrets set "TestSecret:ApiKey" "dev-local-secret-12345" --project src/Backend/TaskFy.Api
```

### 1.3 List Configured Local Secrets
```powershell
dotnet user-secrets list --project src/Backend/TaskFy.Api
```
*Expected Output:*
```text
TestSecret:ApiKey = dev-local-secret-12345
```

### 1.4 Audit Source-Controlled Files
Run git status to verify zero modification to `appsettings.json` or `appsettings.Development.json`:
```powershell
git diff src/Backend/TaskFy.Api/appsettings.json src/Backend/TaskFy.Api/appsettings.Development.json
```
*Expected Output: Empty diff (no secrets committed).*

---

## Scenario 2: Cross-Origin Resource Sharing (CORS) Verification

### 2.1 Start the API in Development
```powershell
dotnet run --project src/Backend/TaskFy.Api --environment Development
```

### 2.2 Verify Preflight `OPTIONS` Request
Dispatch an `OPTIONS` preflight request simulating a browser call from Angular (`http://localhost:4200`):
```powershell
curl -i -X OPTIONS http://localhost:5000/health `
  -H "Origin: http://localhost:4200" `
  -H "Access-Control-Request-Method: GET" `
  -H "Access-Control-Request-Headers: Content-Type,Authorization"
```
*Expected Output:*
- HTTP Status: `200 OK` or `204 No Content`
- `Access-Control-Allow-Origin: *`
- `Access-Control-Allow-Methods: *` or list containing `GET`
- `Access-Control-Allow-Headers: *` or requested headers

### 2.3 Verify Actual Cross-Origin `GET` Request
Dispatch a cross-origin `GET` request:
```powershell
curl -i -X GET http://localhost:5000/health `
  -H "Origin: http://localhost:4200"
```
*Expected Output:*
- HTTP Status: `200 OK`
- `Access-Control-Allow-Origin: *`
- JSON payload containing `status: "Healthy"`

---

## Scenario 3: Automated Test Execution

Execute the automated test suite across unit tests and integration tests:
```powershell
dotnet test src/Backend/TaskFy.sln
```
*Expected Outcome:* All tests pass (including new CORS integration tests in `TaskFy.IntegrationTests`), with 0 warnings and 0 failures.
