# 🎓 School Portal

A multi-service school management system built with **ASP.NET Core 8 MVC** and **Docker**. The system consists of two independent services that communicate over HTTP inside a Docker network, backed by a shared **SQL Server** container.

---

## 📌 Overview

| Service | Description | Port |
|---|---|---|
| **Students App** | Manage students (CRUD) + exposes JSON API | `5001` |
| **Grades App** | Manage grades + fetches student names from Students App | `5002` |
| **SQL Server** | Shared DB container with two separate databases | `1433` |

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│              Docker Network                  │
│                                             │
│  ┌─────────────────┐   HTTP (port 80)       │
│  │   Grades App    │ ──────────────────►    │
│  │   (port 5002)   │   students-mvc         │
│  └────────┬────────┘                        │
│           │                                 │
│  ┌────────▼────────┐   ┌─────────────────┐  │
│  │  Students App   │   │   SQL Server    │  │
│  │   (port 5001)   │──►│  StudentsDb     │  │
│  └─────────────────┘   │  GradesDb       │  │
│                        └─────────────────┘  │
└─────────────────────────────────────────────┘
```

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8 MVC
- **ORM:** Entity Framework Core 8
- **Database:** SQL Server 2022 (Docker)
- **Containerization:** Docker + Docker Compose
- **Frontend:** Razor Views + Bootstrap 5 + Bootstrap Icons

---

## 📁 Project Structure

```
SchoolPortal/
├── StudentsApp/
│   ├── Controllers/
│   │   └── StudentsController.cs     # CRUD + JSON API endpoints
│   ├── Models/
│   │   └── Student.cs
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Views/Students/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Delete.cshtml
│   │   └── Details.cshtml
│   ├── Dockerfile
│   └── appsettings.json
│
├── GradesApp/
│   ├── Controllers/
│   │   └── GradesController.cs
│   ├── Models/
│   │   └── Grade.cs
│   ├── ViewModels/
│   │   └── GradeViewModel.cs
│   ├── Services/
│   │   └── StudentsServiceClient.cs  # HTTP client to Students App
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Views/Grades/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Delete.cshtml
│   │   └── Details.cshtml
│   ├── Dockerfile
│   └── appsettings.json
│
└── docker-compose.yml
```

---

## 🚀 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running
- That's it — no need to install .NET or SQL Server locally

### Run the project

```bash
# Clone the repo
git clone https://github.com/your-username/school-portal.git
cd school-portal

# Build and start all containers
docker compose up --build
```

Then open your browser:

- **Students App →** http://localhost:5001
- **Grades App →** http://localhost:5002

### Stop the project

```bash
docker compose down
```

> Data is persisted in a Docker named volume — it won't be lost when containers stop.

---

## ⚙️ Environment Variables

These are set automatically by `docker-compose.yml`:

| Variable | Service | Description |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Both | SQL Server connection string |
| `StudentsService__BaseUrl` | Grades App | URL of the Students App inside Docker network |
| `ACCEPT_EULA` | SQL Server | Required by Microsoft SQL Server image |
| `SA_PASSWORD` | SQL Server | SA user password |

---

## 🔌 Students App — JSON API

The Students App exposes two endpoints used internally by the Grades App:

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/Students/GetAll` | Returns all students as JSON |
| `GET` | `/Students/GetById/{id}` | Returns a single student or 404 |

---

## 🐳 Docker Details

- Each app uses a **multi-stage Dockerfile** (SDK for build → Runtime for final image)
- **EF Migrations run automatically** on startup via `context.Database.Migrate()`
- The Grades App communicates with the Students App using the Docker service name `students-mvc` (not `localhost`)
- If the Students App is down, the Grades App handles it gracefully with a fallback message instead of crashing

---

## 📝 Notes

- SA password must be strong (uppercase + lowercase + number + symbol) or SQL Server container will fail silently
- Inside Docker, always use the service name as the hostname, never `localhost`
- `depends_on` in Compose ensures startup order but not full SQL Server readiness — the app retries on startup

---

## 📄 License

This project was built as a learning exercise for Docker container-to-container communication with ASP.NET Core.
