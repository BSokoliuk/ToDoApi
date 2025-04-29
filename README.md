# ToDo API

A simple ToDo API built with .NET, featuring Docker support, integration/unit tests, and performance tests.

---

## Features

- **Backend**: ASP.NET Core API.
- **Database**: PostgreSQL.
- **Testing**:
  - Unit Tests
  - Integration Tests
  - Performance Tests using NBomber.
- **Dockerized**: Includes containers for the backend, database, and migrations.

---

## Prerequisites

- [Docker](https://www.docker.com/) installed and running.
- [.NET SDK](https://dotnet.microsoft.com/) installed (for running tests and performance tests locally).

---

## Getting Started

### 1. Run the Application with Docker

The project includes a `docker-compose.yml` file to set up the backend, database, and migrations.

To build and run the application:

```bash
docker-compose up --build
```

This will:

- Build and run the backend API on [http://localhost:5000](http://localhost:5000).
- Set up a PostgreSQL database on `localhost:5432`.
- Apply database migrations using the migrations container.

---

### 2. Running Tests

#### Unit and Integration Tests

To run all unit and integration tests:

```bash
dotnet test
```

This will:

- Execute unit tests located in the `Application.Tests.Unit` project.
- Execute integration tests located in the `API.Tests.Integration` project.

#### Performance Tests

To run performance tests:

1. Ensure the backend and database are running (e.g., via `docker-compose up`).
2. Run the performance tests:
   ```bash
   dotnet run --project API.Tests.Performance/API.Tests.Performance.csproj
   ```

This will execute performance tests using NBomber and generate a performance report.

---

## Project Structure

- **API**: The main backend project.
- **Application**: Contains business logic and service interfaces.
- **Domain**: Contains domain entities and enums.
- **Infrastructure**: Handles database context, repositories, and migrations.
- **API.Tests.Integration**: Integration tests for the API.
- **Application.Tests.Unit**: Unit tests for the application layer.
- **API.Tests.Performance**: Performance tests using NBomber.

---

## Notes

- The database connection string is configured in the `docker-compose.yml` file and the `appsettings.json` file in the API project.
- Ensure the database is running before executing integration or performance tests.
- Performance test results are saved as reports (e.g., `TodoItemPerformanceReport.html`).

---

## Troubleshooting

- If the database is not accessible, ensure the `db` container is running and the connection string matches the database configuration.
- For performance tests, ensure the backend API is running on [http://localhost:5000](http://localhost:5000).
