# Billing Service Microservice (Proof of Concept)

## Overview

This project implements a **billing microservice** designed to simulate the core components of a policy billing engine used in insurance systems. It demonstrates production-ready patterns for scalable, maintainable APIs in transaction-sensitive domains like insurance billing and collections.

The service exposes REST APIs that allow consumers to:
- Retrieve premium schedules for insurance policies
- Record payment attempts and their results
- Identify delinquent policies
- Trigger retries for failed payment attempts

Built with a clean architecture, the BillingService API separates concerns through layered design: Controllers for API exposure, Services for business logic, Repositories for data access, and comprehensive unit testing for reliability. Key features include persistence with PostgreSQL, background retry processing, structured logging with correlation IDs, health checks, and containerized deployment.

---

## System Design Summary

The microservice simulates a billing engine responsible for **tracking policy premiums, payment attempts, and delinquency status**.

### Key Responsibilities
- Manage policy premium schedules
- Record payment attempts and outcomes
- Track delinquent policies
- Retry failed payments
- Simulate integration with an external payment processor

### Architecture

Client applications interact with the **Billing Service API**, which handles business logic and stores data in PostgreSQL via a repository pattern for clean data access.

```
Client
   |
   v
BillingService.Api
   |
   +--> Controllers (API layer)
   |
   +--> Services (business logic)
   |
   +--> Repositories (data access)
   |
   +--> Entity Framework Core
   |
   v
PostgreSQL Database
   |
   v
Background Worker (PaymentRetryWorker)
```

A background worker periodically scans for failed payments and triggers retry logic.

### Key Architectural Decisions
- **Repository Pattern**: Data access logic (e.g., DbContext calls) is abstracted into `IPaymentPolicyRepository` to decouple services from persistence. This supports unit testing with mocks and allows future database changes without affecting business logic.
- **Service Layer**: Handles business rules, logging, and error handling. Services inject repositories and loggers for observability.
- **Error Handling**: Controllers and services use try-catch for graceful failures, returning structured responses (e.g., 500 for exceptions).
- **Testing**: Unit tests cover services, controllers, and repositories, using Moq for mocking dependencies. Tests ensure isolation and verify logging/calls.
- **Background Processing**: `PaymentRetryWorker` runs asynchronously to handle retries without blocking API requests.

---

## Technology Stack

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core** (ORM with PostgreSQL)
- **PostgreSQL** (Database)
- **Docker / Docker Compose** (Containerization)
- **Swagger / OpenAPI** (API Documentation)
- **Background Worker Services** (Hosted services)
- **xUnit, Moq, FluentAssertions** (Unit Testing)
- **Serilog/NLog** (Structured Logging)

---

# Features Demonstrated

## Features Demonstrated

This project showcases:
- ✅ Microservice architecture with layered design
- ✅ REST APIs with OpenAPI documentation
- ✅ PostgreSQL persistence via repository pattern
- ✅ Background retry processing
- ✅ Containerized deployment
- ✅ Structured logging with correlation IDs
- ✅ Health checks for service monitoring
- ✅ Policy delinquency tracking
- ✅ Comprehensive unit testing (services, controllers, repositories)

---

# Observability and Operational Readiness

## Observability and Operational Readiness

The service uses **structured logging with correlation IDs** (via `CorrelationIdMiddleware`) to trace requests across the system. Logs include request IDs for debugging distributed calls.

Health check endpoints (`/health/live`, `/health/ready`) expose service and dependency status (e.g., PostgreSQL connectivity) for orchestration platforms like Kubernetes.

Logs are structured for aggregation tools to generate metrics such as:
- Payment failure rates
- Retry success ratios
- Delinquent policy counts

---

## Database Design

The service uses **Entity Framework migrations** to manage schema evolution. Core tables (from #codebase migrations):

| Table            | Purpose                               | Key Fields                          |
| ---------------- | ------------------------------------- | ----------------------------------- |
| Policies         | Stores policy records                 | Id, CustomerName, Premium           |
| PremiumSchedules | Defines premium due dates and amounts | PolicyId, PolicyNumber, Status, DueDate, Amount |
| PaymentAttempts  | Tracks payment attempts and results   | Id, PolicyId, Amount, AttemptDate, Success, RetryCount |

Seed data (via `DbSeeder`) populates sample policies, schedules, and attempts on startup.

---

## Testing Strategy

Unit tests ensure code reliability:
- **Service Tests**: Mock `IPaymentPolicyRepository` and `ILogger` to test business logic (e.g., payment processing, retries).
- **Controller Tests**: Mock services to verify API responses, logging, and error handling.
- **Repository Tests**: (Future) Integration tests with in-memory DbContext for data access.
- **Coverage**: Targets 80%+ coverage; run with `dotnet test` in the test project.

Tests use xUnit for assertions, Moq for mocking, and FluentAssertions for readable checks.

---

## Running the Project

### Prerequisites
- **Docker** (v20+) and **Docker Compose** (v2+)
- **Git** (for cloning)
- (Optional for local dev) **.NET 8 SDK**, **VS Code** or similar IDE

### Step-by-Step Setup After Cloning
1. **Clone the Repository**

2. **Start the Environment**:
- Ensure Docker is running.
- Build and start containers:
  ```
  docker compose up --build
  ```
- This launches:
  - API container (BillingService.Api) on `http://localhost:5000`
  - PostgreSQL container (paymentapi-db-1) on port 5432
- Wait for logs to show "Postgres ready" and seeding completion (may take 10-20 seconds).

3. **Verify Startup**:
- API: Visit `http://localhost:5000/swagger` for interactive docs.
- Health: Check `http://localhost:5000/health` (should return "Healthy").
- Database: Inspect with `docker exec -it paymentapi-db-1 psql -U billing -d billingdb`, then run `\dt` to list tables (PaymentAttempts, Policies, PremiumSchedules).

4. **Test Endpoints**:
- Use Swagger or tools like Postman/curl.
- Example: POST to `/payments/attempt` with sample JSON.

5. **Stop/Restart**:
```
docker compose down
docker compose up --build # Rebuilds if code changed
```

### Troubleshooting
- **Port Conflicts**: Ensure 5000/5432 are free; adjust in `docker-compose.yml` if needed.
- **Database Issues**: If seeding fails, check logs for "Postgres not ready" – wait longer or restart. Reset DB with `docker compose down -v` to clear volumes.
- **Build Errors**: Run `dotnet build` locally to debug. Ensure .NET 8 SDK is installed.
- **Slow Startup**: PostgreSQL initialization can take time; monitor with `docker logs paymentapi-db-1`.
- **Testing Locally**: Without Docker, set connection strings in `appsettings.json` and run `dotnet run` (ensure PostgreSQL is running locally).

---

## API Endpoints

### Health


| Method | Endpoint | Description |
| :--- | :--- | :--- |
| GET | `/health` | Basic health check (returns "Healthy") |
| GET | `/health/live` | Liveness probe (service status) |
| GET | `/health/ready` | Readiness probe (dependencies) |

**Logic Decisions:**
Simple checks for service availability. `/health/ready` verifies PostgreSQL connectivity via health checks. No auth required; used by orchestrators for load balancing.

### Payments


| Method | Endpoint | Description |
| :--- | :--- | :--- |
| POST | `/payments/attempt` | Record a payment attempt |
| POST | `/payments/retry/{paymentId}` | Retry a failed payment |

**Example Request (for /payments/attempt):**
```json
{
  "policyId": "uuid",
  "amount": 100.00,
  "shouldSucceed": true
}
```

**Logic Decisions (PaymentsController):**
* **Attempt:** Validates request, logs with correlation ID, calls `PaymentService.ProcessPaymentAsync` to create/update `PaymentAttempt`. Returns `PaymentResponse` with success/attempt ID. Errors (e.g., DB issues) return 500.
* **Retry:** Fetches `PaymentAttempt` by ID, increments `RetryCount`, updates success. Returns `PaymentResponse` or 404 if not found. Logs failures; handles exceptions gracefully.

### Policy Billing


| Method | Endpoint | Description |
| :--- | :--- | :--- |
| GET | `/payments/{policyId}/premium-schedule` | Retrieve premium schedule |
| GET | `/payments/all-premium-schedules` | Retrieve all premium schedules |
| GET | `/payments/delinquent` | List delinquent policies |

**Logic Decisions (PoliciesController/AdminController):**
* **Premium Schedule:** Calls `PoliciesService.GetPremiumSchedule` to fetch `PremiumSchedule` by ID. Returns `PolicyResponse` (success + data) or 404/error.
* **All Schedules:** Calls `AdminService.GetAllPremiumSchedulesAsync`. Returns 200 or 500 on error.
* **Delinquent:** Calls `AdminService.GetDelinquentPoliciesAsync` for `DelinquentPolicyDto` list (filtered by status). Returns 200 or 500.

### Seed Data
A simple database seed script automatically inserts sample data on startup so the API has test data available immediately.

Seed data includes:
* Sample policies
* Premium schedules
* Payment attempt records

This allows endpoints to return meaningful results without manual setup.

### Payment Retry Logic
Failed payments are stored in the `PaymentAttempts` table. Retry logic can be triggered in two ways:

1. **Manual Retry**
```http
POST /payments/retry/{paymentId}
```
Increments `RetryCount`, simulates success/failure.

2. **Background Retry Worker**
The `PaymentRetryWorker` (hosted service) scans for failed payments (`Success = false`, `RetryCount < 3`) and retries automatically. Uses `RetryService` for logic.

### Health Monitoring
The service exposes health endpoints using ASP.NET health checks. 

**Dependencies monitored:**
* PostgreSQL database connectivity

This allows orchestration platforms (Docker/Kubernetes) to monitor service health.

### Project Structure
```text
BillingService.Api
│
├── Controllers
│   ├── PaymentsController.cs
│   ├── PoliciesController.cs
│   ├── AdminController.cs
│   └── HealthcheckController.cs
│
├── Services
│   ├── PaymentService.cs
│   ├── PoliciesService.cs
│   ├── AdminService.cs
│   └── RetryService.cs
│
├── Repositories
│   ├── IPaymentPolicyRepository.cs
│   └── PaymentPolicyRepository.cs
│
├── Models
│   ├── Policy.cs
│   ├── PremiumSchedule.cs
│   └── PaymentAttempt.cs
│
├── DTO
│   ├── PaymentRequest.cs
│   ├── PaymentResponse.cs
│   ├── PolicyResponse.cs
│   └── DelinquentPolicyDto.cs
│
├── Data
│   ├── BillingDbContext.cs
│   └── DbSeeder.cs
│
├── Workers
│   └── PaymentRetryWorker.cs
│
├── Middleware
│   └── CorrelationIdMiddleware.cs
│
├── Tests (BillingService.Api.Tests)
│   ├── ControllerTests
│   │   ├── PaymentsControllerTests.cs
│   │   ├── PoliciesControllerTests.cs
│   │   └── AdminControllerTests.cs
│   └── ServiceTests
│       ├── PaymentServiceTest.cs
│       ├── PoliciesServiceTests.cs
│       ├── AdminServiceTest.cs
│       └── RetryServiceTests.cs
│
└── Migrations
```

### Development Notes
* **Swagger:** Interactive UI at `/swagger` for testing endpoints.
* **Tools:** Compatible with Thunder Client, Postman, or curl.
* **Correlation IDs:** Automatically added to logs via middleware for tracing.
* **Migrations:** Run `dotnet ef migrations add <Name>` for schema changes.
* **Seeding:** Controlled by `shouldPersistData` in `Program.cs`; resets data on restart.

### Deliverables
This project demonstrates:
* Microservice architecture with repository pattern
* Billing domain modeling and retry logic
* API design with error handling
* Containerized deployment and unit testing

### License
This project is for demonstration and evaluation purposes.
