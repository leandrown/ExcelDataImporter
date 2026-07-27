# Excel Data Importer API

A REST API built with ASP.NET Core for uploading, validating, and persisting
data from Excel files (`.xlsx`) into a SQL Server database.

Inspired by real-world data ingestion workflows, this project demonstrates a
practical backend solution for structured data intake — with layered
architecture, structured logging, health checks, and automated tests.

---

## Features

- Upload `.xlsx` files via REST endpoint
- Row-level validation with detailed error reporting
- Data persistence via Entity Framework Core (Code First)
- SQL Server / Azure SQL compatible
- Full Swagger/OpenAPI documentation via Scalar
- Structured logging with Serilog (console + rolling file sinks)
- Health checks for SQL Server and the EF Core `DbContext`
- Automated test suite (xUnit)
- PowerShell scripts for database backup and automated import triggering

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 |
| Database | SQL Server / Azure SQL |
| Excel parsing | ClosedXML |
| API documentation | Scalar (OpenAPI) |
| Logging | Serilog |
| Health checks | Microsoft.Extensions.Diagnostics.HealthChecks + AspNetCore.HealthChecks.SqlServer |
| Testing | xUnit, Moq, FluentAssertions |
| Automation | PowerShell 7 |

---

## Project Structure

```
src/
├── ExcelDataImporter.API             → Controllers, Program.cs, health checks
├── ExcelDataImporter.Application      → Services, DTOs, Helpers
├── ExcelDataImporter.Domain           → Entities, Enums, Validators (business rules)
└── ExcelDataImporter.Infrastructure   → EF Core, DbContext, Repositories

tests/
└── ExcelDataImporter.Tests
    ├── Domain/Validators               → Unit tests for business validation rules
    ├── Application/Services            → Unit tests for the import service (mocked)
    └── TestHelpers                     → Shared test data builders

scripts/                                → PowerShell automation scripts
```

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (local or Azure SQL)
- PowerShell 7+
- Visual Studio 2026 (recommended) or any editor with .NET 10 support

### Setup

1. Clone the repository

   ```
   git clone https://github.com/leandrown/ExcelDataImporter.git
   ```

2. Configure the connection string using **User Secrets** (recommended for local development)

   ```
   dotnet user-secrets init --project src/ExcelDataImporter.API
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=ExcelDataImporterDb;User Id=sa;Password=YOUR_PASSWORD;Persist Security Info=True;Encrypt=True;TrustServerCertificate=True;" --project src/ExcelDataImporter.API
   ```

   > The connection string is intentionally **not** stored in `appsettings.Development.json`.
   > Keeping credentials out of source control avoids accidental exposure in the repository history.

3. Apply migrations

   ```
   dotnet ef database update --project src/ExcelDataImporter.Infrastructure --startup-project src/ExcelDataImporter.API
   ```

4. Run the API

   ```
   dotnet run --project src/ExcelDataImporter.API
   ```

5. The API opens automatically in the **Scalar** documentation UI:

   ```
   https://localhost:{port}/scalar/v1
   ```

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/import` | Upload and import an Excel file |
| GET | `/api/import` | List all import operations |
| GET | `/api/import/{id}/records` | Get all records from a specific import |
| GET | `/api/import/{id}/errors` | Get only the rows that failed validation |
| GET | `/api/health` | Detailed health check (SQL Server + EF Core `DbContext`) |
| GET | `/api/health/live` | Lightweight liveness check (no dependency checks) |

---

## Running Tests

```
dotnet test
```

The test suite covers:

- **Domain validation rules** — isolated, dependency-free unit tests for row validation logic
- **Import service behavior** — mocked repository and logger, covering success, partial failure,
  and full failure scenarios, plus repository interaction verification

---

## Logging

Logging is handled by **Serilog**, writing to both the console and a rolling daily file:

```
src/ExcelDataImporter.API/Logs/excel-importer-YYYYMMDD.log
```

Log configuration (minimum levels, sinks, retention) lives in `appsettings.json` /
`appsettings.Development.json` and can be adjusted per environment.

---

## Health Checks

The API exposes two health check endpoints, following common liveness/readiness
conventions used in containerized environments:

- **`/api/health`** — checks SQL Server connectivity directly and through the EF Core
  `DbContext`, returning a detailed JSON report per dependency
- **`/api/health/live`** — confirms the process is running, without touching any
  external dependency

---

## PowerShell Scripts

### Backup database

```
scripts/backup-database.ps1
```

### Trigger import via API

```
scripts/trigger-import.ps1 -FilePath "data.xlsx" -ApiUrl "https://localhost:5001"
```

---

## Roadmap

- [ ] Docker support (API + SQL Server via Docker Compose)
- [ ] CI pipeline with GitHub Actions (build + test on every push)
- [ ] Integration tests with `WebApplicationFactory` and Testcontainers

---

## License

MIT — see [LICENSE](LICENSE) for details.