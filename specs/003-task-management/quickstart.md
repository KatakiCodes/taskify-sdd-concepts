# Quickstart & Verification Guide: Task Creation & Query Management

## Overview
This guide provides runnable instructions to create and query tasks on the TaskFy API in the local development environment.

---

## Scenario 1: Start the API
In development mode, EF Core migrations automatically create and update the local SQLite database (`taskfy.db`):
```powershell
dotnet run --project src/Backend/TaskFy.Api --environment Development
```

---

## Scenario 2: Create a New Task (POST /tasks)

### 2.1 Happy Path - Create Task with End Date
```powershell
curl -i -X POST http://localhost:5000/tasks `
  -H "Content-Type: application/json" `
  -d '{
    "name": "Implementar CQRS no TaskFy",
    "description": "Criar comandos, queries e handlers desacoplados",
    "startDate": "2026-10-15T10:00:00Z",
    "endDate": "2026-10-20T18:00:00Z"
  }'
```
*Expected Output:*
- HTTP Status: `201 Created`
- Header: `Location: /tasks/<generated-guid>`
- Body: JSON representing the task with status `"Pending"` and generated `id`.

### 2.2 Create Task without End Date (Open-Ended)
```powershell
curl -i -X POST http://localhost:5000/tasks `
  -H "Content-Type: application/json" `
  -d '{
    "name": "Estudar Angular 20 Signals",
    "description": "Aprofundar nos novos conceitos reativos",
    "startDate": "2026-10-16T09:00:00Z"
  }'
```
*Expected Output:*
- HTTP Status: `201 Created`
- Property: `"endDate": null`

### 2.3 Validation Failure - Missing Name
```powershell
curl -i -X POST http://localhost:5000/tasks `
  -H "Content-Type: application/json" `
  -d '{
    "startDate": "2026-10-16T09:00:00Z"
  }'
```
*Expected Output:*
- HTTP Status: `400 Bad Request`
- Content-Type: `application/problem+json`
- RFC 7807 payload with validation errors for `name`.

---

## Scenario 3: Get Task by ID (GET /tasks/{id})

```powershell
curl -i -X GET http://localhost:5000/tasks/<task-id>
```
*Expected Output:*
- HTTP Status: `200 OK`
- JSON payload containing the requested task's details.

### 3.1 Non-Existent ID
```powershell
curl -i -X GET http://localhost:5000/tasks/00000000-0000-0000-0000-000000009999
```
*Expected Output:*
- HTTP Status: `404 Not Found`
- Content-Type: `application/problem+json`

---

## Scenario 4: Search & Filter Tasks (GET /tasks)

### 4.1 Filter by Search Term (Name / Description)
```powershell
curl -i -X GET "http://localhost:5000/tasks?searchTerm=CQRS"
```

### 4.2 Filter by Status
```powershell
curl -i -X GET "http://localhost:5000/tasks?status=Pending"
```

### 4.3 Filter by Creation Date Range
```powershell
curl -i -X GET "http://localhost:5000/tasks?createdFrom=2026-10-15T00:00:00Z&createdTo=2026-10-15T23:59:59Z"
```

---

## Scenario 5: Automated Test Execution

Execute the full test suite to validate all unit, architecture, and integration tests:
```powershell
dotnet test src/Backend/TaskFy.sln
```
*Expected Outcome:* All tests pass with 0 failures and 0 compiler warnings.
