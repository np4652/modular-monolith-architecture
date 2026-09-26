# 🏗️ .NET Modular Monolith Architecture with ASP.NET Core & Razor Pages

A production-oriented **Modular Monolith architecture for ASP.NET Core and .NET 10**, designed for building maintainable, scalable, and enterprise-ready web applications with strong module boundaries.

This project demonstrates how to structure a **.NET Modular Monolith** using **ASP.NET Core Razor Pages, Domain-Driven Design (DDD), layered architecture, PostgreSQL, Redis, RabbitMQ, Serilog, OpenTelemetry, permission-based authorization, rate limiting, and Docker**.

The goal is to keep the application as **one deployable ASP.NET Core application** while maintaining clear boundaries between business modules so the system can evolve without immediately introducing the operational complexity of microservices.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet\&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-10.0-512BD4?logo=dotnet\&logoColor=white)](https://learn.microsoft.com/aspnet/core/)
[![C#](https://img.shields.io/badge/C%23-Latest-239120?logo=csharp\&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql\&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis\&logoColor=white)](https://redis.io/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3-FF6600?logo=rabbitmq\&logoColor=white)](https://www.rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker\&logoColor=white)](https://www.docker.com/)

---

## 📌 Overview

Building a large application as a traditional monolith can eventually lead to tightly coupled business logic, shared infrastructure, difficult testing, and unclear ownership.

On the other hand, moving to microservices too early can introduce unnecessary distributed-system complexity.

This project demonstrates a third approach:

> **A Modular Monolith — one application, multiple strongly isolated business modules.**

The application is deployed as a **single ASP.NET Core application**, while each business module owns its Domain, Application, Infrastructure, and Presentation concerns.

### Architecture at a glance

```text
                    ┌───────────────────────────┐
                    │      ASP.NET Core App     │
                    │       .NET 10 / C#        │
                    └─────────────┬─────────────┘
                                  │
             ┌────────────────────┴────────────────────┐
             │                                         │
     ┌───────▼────────┐                       ┌────────▼────────┐
     │ User Management │                       │ Future Modules  │
     │     Module      │                       │                 │
     └───────┬────────┘                       └────────┬────────┘
             │                                         │
      ┌──────▼───────┐                         ┌───────▼────────┐
      │ Presentation  │                         │ Presentation   │
      │ Razor Pages   │                         │                │
      ├───────────────┤                         ├────────────────┤
      │ Application   │                         │ Application    │
      ├───────────────┤                         ├────────────────┤
      │ Domain        │                         │ Domain         │
      ├───────────────┤                         ├────────────────┤
      │ Infrastructure│                         │ Infrastructure │
      └───────┬───────┘                         └───────┬────────┘
              │                                         │
              └────────────────┬────────────────────────┘
                               │
             ┌─────────────────┼──────────────────┐
             │                 │                  │
        PostgreSQL           Redis             RabbitMQ
```

---

## ✨ Key Features

* 🧩 **Modular Monolith Architecture**
* 🏗️ **ASP.NET Core .NET 10**
* 🎯 **Domain-Driven Design principles**
* 📦 Strong business module boundaries
* 🧱 Domain / Application / Infrastructure / Presentation separation
* 🖥️ **ASP.NET Core Razor Pages**
* 🔐 Authentication and permission-based authorization
* 👥 User, role, and permission management
* 🛡️ ASP.NET Core antiforgery protection
* 🚦 Centralized rate limiting
* 🗄️ **Entity Framework Core**
* 🐘 **PostgreSQL**
* ⚡ **Redis distributed caching**
* 🐇 **RabbitMQ event messaging**
* 📡 In-process domain events
* 📝 **Serilog structured logging**
* 🔭 **OpenTelemetry observability**
* 🆔 Correlation ID middleware
* ⚠️ Centralized exception handling
* 🐳 Docker and Docker Compose support
* 🧪 xUnit-based unit testing
* ✅ FluentValidation
* 🔑 BCrypt password hashing
* 🔄 Automatic EF Core database migrations
* 🌱 Database seeding for roles and administrator accounts

The application's composition root wires the shared infrastructure and User Management module together, including Redis, RabbitMQ, authorization, antiforgery, rate limiting, OpenTelemetry, and Razor Pages.

---

# 🏛️ Architecture

## Modular Monolith

Unlike a traditional monolith where everything tends to become interconnected, this project organizes functionality around **business modules**.

Each module owns its complete application stack:

```text
Modules/
└── UserManagement/
    ├── Domain/
    ├── Application/
    ├── Infrastructure/
    └── Presentation/
```

The dependency direction is intentionally controlled:

```text
Presentation
     ↓
Application
     ↓
Domain
     ↑
Infrastructure
```

The Domain layer remains independent from frameworks and infrastructure technologies.

This makes the architecture easier to understand, test, maintain, and evolve.

---

## 🧩 Module Architecture

A module is a complete business capability rather than simply a technical layer.

For example:

```text
UserManagement
│
├── Domain
│   ├── Entities
│   ├── ValueObjects
│   ├── Events
│   ├── Exceptions
│   ├── Repositories
│   └── Enums
│
├── Application
│   ├── DTOs
│   ├── ViewModels
│   ├── Services
│   ├── Validators
│   ├── Pipelines
│   └── EventHandlers
│
├── Infrastructure
│   ├── Persistence
│   ├── Repositories
│   ├── Services
│   └── Messaging
│
└── Presentation
    ├── Pages
    ├── Components
    └── Partials
```

This keeps business functionality together instead of scattering it across global folders such as:

```text
Controllers/
Services/
Repositories/
Models/
Views/
```

---

# 👥 User Management Module

The current application includes a dedicated **User Management module**.

The module is responsible for functionality around:

* Users
* Roles
* Permissions
* Authentication
* Authorization integration
* Password management
* Account lifecycle
* Account status
* Refresh-token support where required

The application registers the module through the composition root:

```csharp
builder.Services.AddUserManagementModule(
    builder.Configuration);
```

This keeps module registration encapsulated and allows additional modules to be introduced without turning `Program.cs` into a large collection of implementation details.

---

# 🖥️ Razor Pages Architecture

The project uses **ASP.NET Core Razor Pages as its primary presentation architecture**.

The intended request flow is:

```text
Browser
   ↓
Razor Page
   ↓
PageModel
   ↓
Application Service
   ↓
Domain
   ↓
Repository
   ↓
EF Core
   ↓
PostgreSQL
```

PageModels are intentionally kept thin.

They should handle:

* UI input
* Model binding
* Calling application services
* Preparing ViewModels
* Page state
* Authorization
* Redirects

Business rules and persistence logic remain outside the presentation layer.

---

# 📁 Project Structure

```text
modular-monolith-architecture/
│
├── src/
│   └── App/
│       │
│       ├── BuildingBlocks/
│       │
│       ├── Modules/
│       │   └── UserManagement/
│       │       ├── Domain/
│       │       ├── Application/
│       │       ├── Infrastructure/
│       │       └── Presentation/
│       │
│       ├── Migrations/
│       │
│       ├── Pages/
│       │   └── Shared/
│       │
│       ├── wwwroot/
│       │
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.*.json
│
├── tests/
│   └── App.UnitTests/
│
├── ARCHITECTURE.md
├── Dockerfile
├── docker-compose.yml
└── ModularArchitectureWithRazor.slnx
```

The application intentionally uses **one deployable ASP.NET Core web project** while preserving internal module boundaries.

---

# 🧱 Building Blocks

Shared infrastructure is isolated inside `BuildingBlocks`.

Examples include:

```text
BuildingBlocks/
├── Domain/
│
├── Application/
│
└── Infrastructure/
    ├── Authentication/
    ├── Authorization/
    ├── Caching/
    ├── Events/
    ├── Filters/
    ├── Messaging/
    ├── Middleware/
    ├── Observability/
    ├── Options/
    ├── Persistence/
    ├── RateLimiting/
    ├── Security/
    ├── Storage/
    └── Services/
```

Building Blocks are intended for genuinely shared technical capabilities rather than business-specific functionality.

---

# 🗄️ Data & Persistence

The project uses:

* **Entity Framework Core**
* **PostgreSQL**
* Module-specific `DbContext`
* EF Core migrations
* Automatic database migration support
* Database seeders

The current Docker Compose environment provisions PostgreSQL 16 and creates the User Management database.

The application can automatically execute pending migrations during startup when database auto-migration is enabled.

```text
Application
     ↓
UserManagementDbContext
     ↓
Entity Framework Core
     ↓
PostgreSQL
```

---

# ⚡ Redis Caching

Redis is integrated as a shared infrastructure capability.

The application registers:

```text
ICacheService
      ↓
RedisCacheService
      ↓
Redis
```

Redis is provisioned automatically through Docker Compose for local development.

---

# 🐇 RabbitMQ Messaging

RabbitMQ provides messaging infrastructure for asynchronous application communication.

The application exposes an event bus abstraction:

```text
Application / Module
        ↓
      IEventBus
        ↓
 RabbitMQ Event Bus
        ↓
     RabbitMQ
```

RabbitMQ is included in the development Docker Compose environment together with its management interface.

---

# 🔐 Authentication & Authorization

Security is treated as a cross-cutting concern.

The application includes:

* Cookie authentication
* Permission-based authorization
* Authorization policy provider
* Authorization handlers
* Password hashing using BCrypt
* Antiforgery protection
* Server-side authorization
* Permission-protected Razor Page folders

Example:

```csharp
options.Conventions.AuthorizeFolder(
    "/Users",
    PermissionPolicyProvider.PolicyPrefix +
    UserManagementPermissions.UsersView);
```

This allows authorization rules to be expressed in terms of application permissions rather than scattering hard-coded role checks throughout the UI.

---

# 🛡️ Security

State-changing requests use ASP.NET Core antiforgery protection.

The application configures a CSRF header:

```text
X-CSRF-TOKEN
```

Security-sensitive decisions are enforced server-side rather than relying on client-side JavaScript or hidden form fields.

---

# 🚦 Rate Limiting

The application includes centralized rate-limiting infrastructure.

```text
HTTP Request
     ↓
Rate Limiting Middleware
     ↓
Authentication
     ↓
Authorization
     ↓
Application
```

Rate-limiting configuration is exposed through application options, allowing the behavior to be configured without coupling business modules to the underlying infrastructure.

---

# 📡 Observability

The application includes observability capabilities based on **OpenTelemetry**.

Instrumentation includes:

* ASP.NET Core
* HTTP
* OpenTelemetry hosting integration

The project also integrates **Serilog** for application logging.

A correlation ID middleware is also included so requests can be traced consistently across application components.

---

# 📝 Logging

Serilog is configured as the application's logging infrastructure.

The project uses:

```text
Serilog.AspNetCore
Serilog.Enrichers.Environment
Serilog.Sinks.Console
```

This provides structured application logging while keeping logging concerns outside individual business modules.

---

# 🧪 Testing

The repository contains a dedicated unit test project:

```text
tests/
└── App.UnitTests/
```

The test project uses:

* xUnit
* Moq
* FluentValidation
* Entity Framework Core
* Coverlet

It targets .NET 10 and references the main application project.

Run the tests with:

```bash
dotnet test
```

---

# 🐳 Docker Development

The repository includes:

```text
Dockerfile
docker-compose.yml
```

Docker Compose provisions the development infrastructure:

```text
┌─────────────────────────────┐
│          App                │
│       ASP.NET Core          │
│          :8080              │
└──────────────┬──────────────┘
               │
     ┌─────────┼─────────┐
     │         │         │
     ▼         ▼         ▼
 PostgreSQL  Redis   RabbitMQ
   :5432     :6379     :5672
                       :15672
```

The Compose configuration includes health checks and makes the application depend on healthy PostgreSQL, Redis, and RabbitMQ services before startup.

---

# 🚀 Getting Started

## Prerequisites

Install:

* [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* Git

---

## 1. Clone the repository

```bash
git clone https://github.com/np4652/modular-monolith-architecture.git

cd modular-monolith-architecture
```

---

## 2. Start infrastructure with Docker Compose

```bash
docker compose up -d
```

This starts:

* ASP.NET Core application
* PostgreSQL
* Redis
* RabbitMQ

The development application is configured to listen on port `8080`.

---

## 3. Run the application locally

If you prefer to run the ASP.NET Core application directly:

```bash
dotnet restore

dotnet build

dotnet run --project src/App/App.csproj
```

---

## 4. Run tests

```bash
dotnet test
```

---

# ⚙️ Configuration

The application uses standard ASP.NET Core configuration.

Important configuration areas include:

```text
Database
Redis
RabbitMq
RateLimiting
```

For Docker Compose development, the application receives values such as:

```text
ConnectionStrings__UserManagement
Redis__ConnectionString
RabbitMq__HostName
```

from the Compose environment.

---

# 🧠 Why Modular Monolith?

A Modular Monolith can be useful when an application needs strong internal boundaries without immediately becoming a distributed system.

### Traditional Monolith

```text
┌─────────────────────────────────┐
│          Large Monolith         │
│                                 │
│ Users ─ Orders ─ Billing ─ CRM  │
│   └────── Shared Everything ───┘
└─────────────────────────────────┘
```

Over time, boundaries can become difficult to enforce.

### Modular Monolith

```text
┌─────────────────────────────────────────┐
│          Single Application             │
│                                         │
│ ┌──────────┐ ┌──────────┐ ┌──────────┐ │
│ │  Users   │ │ Orders   │ │ Billing  │ │
│ │  Module  │ │  Module  │ │  Module  │ │
│ └──────────┘ └──────────┘ └──────────┘ │
│                                         │
│        Controlled Module Boundaries     │
└─────────────────────────────────────────┘
```

You get:

* One deployment unit
* In-process communication
* Lower operational complexity
* Clear ownership
* Stronger separation of business capabilities
* Easier local development
* A structure that can support future extraction of modules if requirements justify it

This repository deliberately does **not** implement microservices. The architecture specification explicitly defines one deployable application with strict internal module boundaries.

---

# 🔄 Module Communication

Modules should communicate through explicit application contracts and shared infrastructure abstractions rather than directly reaching into another module's implementation.

Conceptually:

```text
Module A
   │
   ├── Application Contract
   │
   └── Event
          ↓
      Event Bus
          ↓
Module B
```

This keeps modules loosely coupled and makes future architectural evolution easier.

---

# 📐 Architecture Principles

This project follows several core architectural principles.

### 1. Strong Module Boundaries

Each business capability owns its:

```text
Domain
Application
Infrastructure
Presentation
```

### 2. Dependency Inversion

Dependencies point toward abstractions and the Domain.

```text
Presentation
      ↓
Application
      ↓
Domain
```

Infrastructure implements abstractions required by the inner layers.

### 3. Framework-Independent Domain

The Domain should not depend directly on:

* ASP.NET Core
* Razor Pages
* Entity Framework Core
* PostgreSQL
* Redis
* RabbitMQ
* HTTP
* JavaScript
* External APIs

### 4. Thin Presentation Layer

Razor PageModels coordinate UI behavior rather than implementing business rules.

### 5. Shared Infrastructure, Isolated Business Logic

Cross-cutting concerns belong in BuildingBlocks.

Business rules belong inside their respective modules.

---

# 🔍 Technology Stack

| Technology                | Purpose                            |
| ------------------------- | ---------------------------------- |
| **C#**                    | Application development            |
| **.NET 10**               | Runtime and platform               |
| **ASP.NET Core**          | Web application framework          |
| **Razor Pages**           | Server-rendered UI                 |
| **Entity Framework Core** | Data access                        |
| **PostgreSQL**            | Relational database                |
| **Redis**                 | Distributed caching                |
| **RabbitMQ**              | Messaging/event infrastructure     |
| **Serilog**               | Structured logging                 |
| **OpenTelemetry**         | Observability                      |
| **FluentValidation**      | Validation                         |
| **BCrypt.Net**            | Password hashing                   |
| **xUnit**                 | Unit testing                       |
| **Moq**                   | Test mocking                       |
| **Docker**                | Containerization                   |
| **Docker Compose**        | Local infrastructure orchestration |

The package references in the application project confirm the core framework and infrastructure stack, including PostgreSQL/EF Core, Redis, RabbitMQ, Serilog, OpenTelemetry, BCrypt, and FluentValidation.

---

# 🎯 Who Is This Project For?

This repository is useful for developers who want to learn or implement:

* **ASP.NET Core Modular Monolith Architecture**
* **.NET 10 application architecture**
* **Razor Pages architecture**
* **Domain-Driven Design in .NET**
* **Clean Architecture principles**
* **Enterprise application architecture**
* **Modular application design**
* **Vertical business module organization**
* **PostgreSQL with Entity Framework Core**
* **Redis caching in ASP.NET Core**
* **RabbitMQ messaging**
* **Permission-based authorization**
* **OpenTelemetry in ASP.NET Core**
* **Dockerized .NET development**

It can also serve as a starting point for enterprise internal applications, administration systems, SaaS back-office applications, and other systems where modularity is important but a distributed microservice architecture is not yet necessary.

---

# 📚 Architecture Documentation

For the detailed architectural rules and design decisions, see:

**[ARCHITECTURE.md](ARCHITECTURE.md)**

The architecture specification covers:

* Modular monolith principles
* Module boundaries
* Dependency rules
* Building Blocks
* Razor Pages architecture
* JavaScript organization
* Security
* User Management
* Persistence
* Messaging
* Caching
* Observability
* Testing
* Infrastructure boundaries

---

# 🛣️ Extending the Application

A new business capability should be introduced as a module.

For example:

```text
Modules/
├── UserManagement/
│
├── Notification/
│
├── Reporting/
│
└── FutureModule/
```

Each module should own its business logic rather than adding unrelated code to shared infrastructure.

A typical new module can follow:

```text
NewModule/
├── Domain/
├── Application/
├── Infrastructure/
└── Presentation/
```

This allows the application to grow horizontally while preserving architectural boundaries.

---

# 🤝 Contributing

Contributions, improvements, architectural discussions, and bug reports are welcome.

Before submitting a pull request:

1. Keep business logic inside the appropriate module.
2. Avoid introducing unnecessary shared dependencies.
3. Preserve dependency direction.
4. Keep Razor PageModels focused on presentation concerns.
5. Add or update tests where appropriate.
6. Avoid moving business-specific code into `BuildingBlocks`.
7. Keep module boundaries explicit.

---

# 📄 License

See the repository for licensing information.

---

# ⭐ Keywords

`ASP.NET Core Modular Monolith` · `.NET 10 Modular Monolith` · `Modular Monolith Architecture` · `.NET Architecture` · `ASP.NET Core Architecture` · `Razor Pages` · `Clean Architecture` · `Domain Driven Design` · `DDD` · `Enterprise Architecture` · `C# Architecture` · `Entity Framework Core` · `PostgreSQL` · `Redis` · `RabbitMQ` · `Serilog` · `OpenTelemetry` · `Docker` · `Razor Pages Architecture` · `Modular Architecture` · `Enterprise ASP.NET Core`

---

## ⭐ If You Find This Useful

If this repository helps you understand or implement **Modular Monolith Architecture with ASP.NET Core**, consider giving it a ⭐ on GitHub and sharing it with other .NET developers interested in scalable application architecture.
