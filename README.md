![.NET](https://img.shields.io/badge/-.NET-blueviolet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?logo=redis&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)
![Swagger](https://img.shields.io/badge/-Swagger-85EA2D?style=flat&logo=swagger&logoColor=white)

# ERP Management API

## Table of Contents

<ol>
  <li><a href="#-overview">Overview</a></li>
  <li><a href="#-technologies">Technologies</a></li>
  <li><a href="#-features">Features</a></li>
  <li>
    <a href="#-getting-started">Getting Started</a>
    <ul>
      <li><a href="#-prerequisites">Prerequisites</a></li>
      <li><a href="#-installation">Installation</a></li>
    </ul>
  </li>
  <li><a href="#-environment-setup">Environment Setup</a></li>
  <li><a href="#-testing">Testing</a></li>
  <li><a href="#-api-documentation">API Documentation</a></li>
</ol>

## 🎯 Overview

ERP Management API is a RESTful backend system built with ASP.NET Core for managing core business operations.

The system provides management capabilities for customers, products, categories, suppliers, purchase orders, sales orders, invoices, employees, departments, and users.

It also implements advanced REST API and enterprise backend patterns including HATEOAS, content negotiation, pagination, search, JWT authentication, refresh tokens, role-based and permission-based authorization, Redis caching, rate limiting, health checks, optimistic concurrency, and automatic audit logging.

The project focuses on building a secure, maintainable, and scalable Web API using reusable infrastructure and established backend patterns.

## 🔧 Technologies

- **.NET / ASP.NET Core** – Framework used to build the RESTful Web API.
- **C#** – Primary programming language.
- **PostgreSQL** – Relational database used for application data.
- **Entity Framework Core** – ORM used for database access and migrations.
- **ASP.NET Core Identity** – User and identity management.
- **JWT** – Token-based authentication.
- **Redis** – Distributed caching for authorization and read operations.
- **FluentValidation** – Request validation.
- **Swagger / OpenAPI** – Interactive API documentation and testing.
- **Hangfire** – Background job processing for asynchronous report exports.
- **CSV Export** – Exporting reports as downloadable CSV files
- **Docker** – Containerization and infrastructure support.
- **.NET Aspire Dashboard** – Application observability and monitoring.
- **GitHub Actions** – CI/CD automation.

## ✨ Features

#### 🧭 API Design

- RESTful API architecture.
- Pagination and search support for customers.
- HATEOAS support.
- Content negotiation for HATEOAS responses.
- Centralized media type handling.
- Problem Details for centralized API error responses.
- Swagger/OpenAPI API documentation.
- CORS support.

#### 👥 Business Management

- Customer management with CRUD operations.
- Customer soft delete support.
- Product management.
- Category management.
- Supplier management.
- Purchase order management.
- Sales order management.
- Invoice management.
- Employee management.
- Department management.
- User management.

#### 📊 Analytics & Reports

- Sales reports with date filtering and grouping by day, week, or month.
- Inventory reports with category filtering and low-stock identification.
- Invoice reports with date-range and status filtering.
- Task progress reports grouped by project, assignee, and status.
- Reports computed from existing business records.
- Asynchronous report exports using Hangfire background jobs.
- CSV report generation and download.
- Background job status tracking.
- Export file expiration after 24 hours.
- Secure export downloads restricted to the requesting user or an Admin.
- Authorization protection for report endpoints.
- Filtering and aggregation for business reporting.

#### 🔐 Security

- ASP.NET Core Identity integration.
- User registration.
- JWT authentication.
- Refresh token authentication.
- Secure refresh token handling.
- Logout endpoint.
- Role management.
- Role-based authorization.
- Permission-based authorization policies.
- User ownership enforcement for orders.

#### ⚡ Performance & Reliability

- Redis caching for authorization and read operations.
- API rate limiting.
- Optimistic concurrency using row versioning.
- Automatic audit logging.
- Infrastructure health checks.
- Background job processing for long-running report exports.

#### 🗄️ Data Access & Infrastructure

- PostgreSQL database integration.
- Entity Framework Core.
- Database migrations.
- Separate Identity migrations.
- Generic Repository pattern.
- Unit of Work pattern.
- Dependency Injection.
- Reusable API and repository infrastructure.

#### 📊 Observability

- .NET Aspire Dashboard integration.
- Infrastructure health monitoring.
- Application observability support.

## 🚀 Getting Started

### 📋 Prerequisites

Make sure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Redis](https://redis.io/download/)
- [Docker](https://www.docker.com/products/docker-desktop/) — if using containerized infrastructure.
- [Git](https://git-scm.com/)

Verify the .NET SDK:

```bash
dotnet --version
```

Verify Docker:

```bash
docker --version
docker compose version
```

### 📥 Installation

1. Clone the repository:

```bash
git clone https://github.com/SeifMohmmed/NexaERP.git
```

2. Navigate to the project:

```bash
cd NexaERP
```

3. Restore dependencies:

```bash
dotnet restore
```

4. Build the project:

```bash
dotnet build
```

5. Apply database migrations:

```bash
dotnet ef database update
```

6. Run the application:

```bash
dotnet run
```

## ⚙️ Environment Setup

The application requires configuration for the database, authentication, caching, and infrastructure services.

### Database

Configure the PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_database;Username=your_user;Password=your_password"
  }
}
```

### JWT Authentication

Configure JWT settings:

```json
{
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "your-api",
    "Audience": "your-client",
    "ExpirationInMinutes": 30
  }
}
```

Sensitive configuration values should be stored using environment variables or ASP.NET Core User Secrets instead of being committed to source control.

### Redis

Configure Redis:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

## 🧪 Testing

The project is structured to support testing of its API and business functionality.

Run the test suite:

```bash
dotnet test
```

## 📘 API Documentation

The API provides interactive documentation through Swagger/OpenAPI.

Once the application is running, open Swagger UI to explore and test the available endpoints.

The documentation covers API resources, authentication requirements, request models, responses, and supported operations.

## 🏗️ Architecture Highlights

The project implements reusable backend patterns and cross-cutting concerns:

- Generic Repository
- Unit of Work
- Dependency Injection
- ASP.NET Core Identity
- JWT Authentication
- Permission-Based Authorization
- HATEOAS
- Content Negotiation
- FluentValidation
- Problem Details
- Redis Caching
- Rate Limiting
- Health Checks
- Optimistic Concurrency
- Audit Logging
- PostgreSQL + EF Core
- Asynchronous CSV Report Exports
- Analytics & Reporting
- Secure Export Downloads
- .NET Aspire Dashboard

## 📌 Project Modules

```text
Customers
Products
Categories
Suppliers
Purchase Orders
Sales Orders
Invoices
Employees
Departments
Users
Analytics & Reports
Background Jobs & Report Exports
Authentication & Authorization
```

### Cross-Cutting Infrastructure

```text
Caching
Validation
Error Handling
Authentication
Authorization
Audit Logging
Concurrency
Rate Limiting
Health Checks
Observability
Background Processing
File Export & Expiration
```
