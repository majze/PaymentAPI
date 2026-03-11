# Billing Service Microservice (Proof of Concept)

## Overview

This project implements a **billing microservice** designed to simulate the core components of a policy billing engine used in insurance systems.

The service exposes REST APIs that allow consumers to:

* Retrieve premium schedules for insurance policies
* Record payment attempts and their results
* Identify delinquent policies
* Trigger retries for failed payment attempts

This implementation demonstrates how a **service-oriented architecture** can support high-availability, transaction-sensitive domains like **insurance billing and collections**.

The project is intentionally simplified but showcases production-style patterns including persistence, background processing, retry mechanisms, structured logging, and containerized deployment.

---

# System Design Summary

The microservice simulates a billing engine responsible for **tracking policy premiums, payment attempts, and delinquency status**.

### Key Responsibilities

* Manage policy premium schedules
* Record payment attempts and outcomes
* Track delinquent policies
* Retry failed payments
* Simulate integration with an external payment processor

### Architecture

Client applications interact with the **Billing Service API**, which handles business logic and stores data in PostgreSQL.

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
   +--> Entity Framework Core
   |
   v
PostgreSQL Database
   |
   v
Background Worker (PaymentRetryWorker)
```

A background worker periodically scans for failed payments and triggers retry logic.

---

# Technology Stack

* **.NET 8**
* **ASP.NET Core Web API**
* **Entity Framework Core**
* **PostgreSQL**
* **Docker / Docker Compose**
* **Swagger / OpenAPI**
* **Background Worker Services**

---

# Features Demonstrated

This project showcases the fundamentals of:

✔ Microservice architecture
✔ REST APIs
✔ PostgreSQL persistence
✔ Background retry processing
✔ Containerized deployment
✔ Structured logging with correlation IDs
✔ Health checks for service monitoring
✔ Policy delinquency tracking

---

# Observability and Operational Readiness

The service uses **structured logging with correlation IDs** so that individual requests can be traced across the system.

Health check endpoints expose service and dependency status for orchestration systems.

Logs are structured so downstream log aggregation platforms can generate operational metrics such as:

* payment failure rates
* retry success ratios
* delinquent policy counts

---

# Database Design

The service uses **Entity Framework migrations** to manage schema evolution.

Core database tables include:

| Table            | Purpose                               |
| ---------------- | ------------------------------------- |
| Policies         | Stores policy records                 |
| PremiumSchedules | Defines premium due dates and amounts |
| PaymentAttempts  | Tracks payment attempts and results   |

---

# Running the Project

## Prerequisites

Install the following:

* **Docker**
* **Docker Compose**
* **Git**

(Optional for local development)

* **.NET 8 SDK**
* **VS Code / Code OSS**

---

# Start the Application

Clone the repository:

```
git clone <repo-url>
cd BillingService
```

Start the system using Docker:

```
docker compose up --build
```

This will start:

* Billing API container
* PostgreSQL database container

Once running, the API will be available at:

```
http://localhost:5000
```

Swagger UI:

```
http://localhost:5000/swagger
```

---

# Restarting the API

To stop and restart the environment:

```
docker compose down
docker compose up --build
```

---

# Inspecting the Database

You can connect directly to the PostgreSQL container.

```
docker exec -it paymentapi-db-1 psql -U billing -d billingdb
```

List tables:

```
\dt
```

You should see:

```
PaymentAttempts
Policies
PremiumSchedules
```

---

# Seed Data

A simple **database seed script** automatically inserts sample data on startup so the API has test data available immediately.

Seed data includes:

* Sample policies
* Premium schedules
* Payment attempt records

This allows endpoints to return meaningful results without manual setup.

---

# API Endpoints

### Health

| Method | Endpoint  | Description                         |
| ------ | --------- | ----------------------------------- |
| GET    | `/health` | Service and dependency health check |

---

### Payments

| Method | Endpoint                      | Description              |
| ------ | ----------------------------- | ------------------------ |
| POST   | `/payments/attempt`           | Record a payment attempt |
| POST   | `/payments/retry/{paymentId}` | Retry a failed payment   |

Example request:

```json
{
  "policyId": "uuid",
  "amount": 100.00,
  "shouldSucceed": true
}
```

---

### Policy Billing

| Method | Endpoint                                | Description                    |
| ------ | --------------------------------------- | ------------------------------ |
| GET    | `/payments/{policyId}/premium-schedule` | Retrieve premium schedule      |
| GET    | `/payments/all-premium-schedules`       | Retrieve all premium schedules |
| GET    | `/payments/delinquent`                  | List delinquent policies       |

---

# Payment Retry Logic

Failed payments are stored in the **PaymentAttempts** table.

Retry logic can be triggered in two ways:

### Manual Retry

```
POST /payments/retry/{paymentId}
```

### Background Retry Worker

The `PaymentRetryWorker` background service periodically scans for failed payments and attempts retries automatically.

---

# Health Monitoring

The service exposes health endpoints using ASP.NET health checks.

Dependencies monitored:

* PostgreSQL database connectivity

This allows orchestration platforms (Docker/Kubernetes) to monitor service health.

---

# Project Structure

```
BillingService.Api
│
├── Controllers
│   ├── PaymentsController
│   ├── PoliciesController
│   └── HealthCheckController
│
├── Services
│   ├── PaymentService
│   └── RetryService
│
├── Models
│   ├── Policy
│   ├── PremiumSchedule
│   └── PaymentAttempt
│
├── Data
│   ├── BillingDbContext
│   └── DbSeeder
│
├── Workers
│   └── PaymentRetryWorker
│
├── Middleware
│   └── CorrelationIdMiddleware
│
└── Migrations
```

---

# Development Notes

Swagger provides an interactive interface for testing all endpoints.

Other OpenAPI-compatible tools can also be used:

* Thunder Client
* Postman
* curl

---

# Deliverables

This project was created as part of a system design and coding exercise demonstrating:

* microservice architecture
* billing domain modeling
* retry logic
* API design
* containerized deployment

The repository contains:

* Source code
* Docker configuration
* Database migrations
* Sample data seed
* API documentation via Swagger

---

# License

This project is intended for demonstration and evaluation purposes.
