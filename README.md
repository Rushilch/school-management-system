# School Management System — Multi-Service Student/Grades System (ASP.NET Core + Docker)

School Management System is a two-service school management system built on ASP.NET Core 8 MVC, containerized with Docker and backed by a shared SQL Server instance. The Students service owns student records and exposes a small internal JSON API; the Grades service owns grade records and calls that API over the Docker network to resolve student names, rather than duplicating student data.

---

## Architecture Overview

The system is two independent ASP.NET Core MVC apps sharing one SQL Server container, each with its own database.

**Request flow for grade views:**
Grades App request → EF Core query against `GradesDb` → `StudentsServiceClient` calls `GET /Students/GetById/{id}` on the Students App (via the `students-mvc` service name, not `localhost`) → student name resolved and merged into the view model → rendered in `Grades/Index.cshtml` or `Details.cshtml`.

**Request flow for student CRUD:**
Students App request → EF Core query/write against `StudentsDb` → standard Razor CRUD views. No outbound calls — this service has no dependency on Grades.

If the Students App is unreachable, `StudentsServiceClient` catches the failure and the Grades App falls back to a placeholder message instead of throwing, so a dependency outage degrades gracefully rather than crashing the request.

---

## Inter-Service Communication

All cross-service calls go through `GradesApp/Services/StudentsServiceClient.cs`, a thin `HttpClient` wrapper around the Students App's internal API. Two endpoints are used:

- **`GET /Students/GetAll`** — returns all students as JSON, used where the Grades App needs to populate a full student list (e.g. Create/Edit dropdowns).
- **`GET /Students/GetById/{id}`** — returns a single student or 404, used to resolve a name for an individual grade record.

These calls are synchronous HTTP requests made from within the Grades App's request pipeline — there's no message queue or event bus between services. At current scale this is fine; under higher load or with more services, this is the seam where you'd introduce a queue (e.g. RabbitMQ) or a shared read-only cache to avoid chatty synchronous calls between containers.

---

## Data Model

Two separate databases in one SQL Server container, one per service, connected through the shared `ConnectionStrings__DefaultConnection` (with each app pointed at its own database name).

- **StudentsDb** — `Student` (id, name, and related profile fields). Owned exclusively by the Students App; the Grades App never queries this database directly, only through the HTTP API.
- **GradesDb** — `Grade` (id, student id, subject, score, and related fields). Owned exclusively by the Grades App. Student identity here is a foreign reference resolved at render time via the API call above, not a local join.

EF Core migrations run automatically on startup (`context.Database.Migrate()`) for both services — no manual migration step required when spinning up fresh containers.

---

## Getting Started

```bash
git clone https://github.com/Rushilch/school-Management System.git
cd school-Management System

docker compose up --build
```

No local .NET or SQL Server install needed — everything runs in containers.

- **Students App →** http://localhost:5001
- **Grades App →** http://localhost:5002

```bash
docker compose down
```

Data persists in a named Docker volume across restarts.

---

## Environment Variables

Set automatically by `docker-compose.yml`:

| Variable | Service | Description |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Both | SQL Server connection string |
| `StudentsService__BaseUrl` | Grades App | URL of the Students App inside the Docker network (`students-mvc`) |
| `ACCEPT_EULA` | SQL Server | Required by the Microsoft SQL Server image |
| `SA_PASSWORD` | SQL Server | SA user password — must be strong or the container fails silently |

---

## Project Structure

```
SchoolManagement System/
├── StudentsApp/
│   ├── Controllers/StudentsController.cs   # CRUD + JSON API endpoints
│   ├── Models/Student.cs
│   ├── Data/ApplicationDbContext.cs
│   ├── Views/Students/                     # Index, Create, Edit, Delete, Details
│   └── Dockerfile
│
├── GradesApp/
│   ├── Controllers/GradesController.cs
│   ├── Models/Grade.cs
│   ├── ViewModels/GradeViewModel.cs
│   ├── Services/StudentsServiceClient.cs   # HTTP client to Students App
│   ├── Data/ApplicationDbContext.cs
│   ├── Views/Grades/                       # Index, Create, Edit, Delete, Details
│   └── Dockerfile
│
└── docker-compose.yml
```

---

## Known Limitations & Design Notes

- **Synchronous inter-service calls:** every name lookup from Grades App to Students App is a blocking HTTP call on the request thread. Fine at current scale; under real load this is where you'd add caching or an async/event-driven handoff.
- **No shared data contract:** the two services agree on a JSON shape by convention, not by a shared schema or generated client. A breaking change to the Students API silently breaks Grades App unless caught in testing.
- **Fallback is a UI message, not a retry:** if the Students App is down, Grades App degrades gracefully but doesn't retry or queue the failed lookup — the student name is just missing until the next request.
- **`depends_on` isn't a readiness check:** Compose starts SQL Server before the apps, but doesn't wait for it to actually accept connections — both apps retry on startup to cover the gap.
- **Hostnames matter inside Docker:** the Grades App must use the service name `students-mvc`, never `localhost`, to reach the Students App — a common first-run mistake.
- **SA password requirements:** must include uppercase, lowercase, a number, and a symbol, or the SQL Server container fails silently with no clear error.

---

## Planned Work

- **Resilience for inter-service calls:** retries with backoff (e.g. Polly) around `StudentsServiceClient`, instead of a single try/fallback.
- **Shared API contract:** a versioned OpenAPI spec or shared DTO package so both services stay in sync as the student model evolves.
- **Async decoupling:** move name resolution off the request path for bulk views (e.g. Grades Index listing many students) via a lightweight cache or background sync, rather than one HTTP call per row.
- **Health checks:** proper `/health` endpoints on both services, surfaced through Docker Compose health checks instead of blind `depends_on`.

---

## Stack

| Layer | Implementation |
|---|---|
| Backend | ASP.NET Core 8 MVC, C# |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2022 (Docker) |
| Inter-service comms | HTTP (`HttpClient`) between containers on the Docker network |
| Frontend | Razor Views · Bootstrap 5 · Bootstrap Icons |
| Containerization | Docker · Docker Compose (multi-stage builds per service) |
