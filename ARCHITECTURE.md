# Enterprise Modular Monolith Architecture Specification

## 1. PURPOSE

Design and implement the application as a **production-grade enterprise ASP.NET Core Modular Monolith**.

The goal is to reproduce the **architectural approach, engineering structure, separation of concerns, design patterns, infrastructure boundaries, and coding standards** of a large enterprise application without reproducing proprietary business logic.

The implementation must remain generic and extensible.

The architecture must support multiple business modules while keeping each module strongly isolated.

**Do not introduce Multi-Tenancy.**

This application is a **single-tenant application**.

---

# 2. ARCHITECTURAL PRINCIPLES

## 2.1 Modular Monolith

Use a **Modular Monolith**, not Microservices.

There must be:

* One deployable ASP.NET Core application.
* One ASP.NET Core Web project.
* One deployment unit.
* Strict internal module boundaries.
* Independent module ownership of Domain, Application, Infrastructure, and Presentation.
* No distributed deployment between modules.

Do NOT create:

* Separate applications per module.
* Separate APIs per module.
* Separate frontend applications.
* Microservices.
* HTTP communication between modules.

The architecture must allow modules to be extracted into services in the future if required, but the current implementation must remain a Modular Monolith.

---

# 3. VERTICAL MODULE ARCHITECTURE

Each business capability must be implemented as an independent module.

Every module follows:

```text
Presentation
     ↓
Application
     ↓
Domain
     ↑
Infrastructure
```

Each module owns its complete stack:

```text
Modules/
└── {ModuleName}/
    ├── Domain/
    ├── Application/
    ├── Infrastructure/
    └── Presentation/
```

Examples:

```text
Modules/
├── UserManagement/
├── Notification/
├── FileManagement/
├── Reporting/
└── {FutureModule}/
```

The actual business modules must be determined from the application requirements.

Do not create unnecessary modules merely for architectural demonstration.

---

# 4. SINGLE-TENANT ARCHITECTURE

## IMPORTANT

This application is **NOT multi-tenant**.

Do not implement any tenant-related architecture.

The following must NOT exist anywhere in the application:

```text
Tenant
TenantId
TenantContext
ITenantContext
TenantResolver
TenantMiddleware
TenantStatus
TenantIsolation
TenantProvisioning
TenantFilter
TenantSelector
Multitenancy
Tenant-aware cache keys
Tenant-aware database filtering
Tenant-aware authorization
Tenant-aware repository logic
```

Do not add:

```text
BuildingBlocks/Infrastructure/Multitenancy/
```

Do not introduce:

```csharp
ITenantContext
```

Do not add `TenantId` to entities unless explicitly required by a future business requirement.

Do not add tenant query filters.

Do not add tenant-specific indexes.

Do not add tenant resolution middleware.

Do not add tenant-specific integration tests.

The database architecture is simply:

```text
Application
    ↓
Module DbContext
    ↓
PostgreSQL
```

There is no tenant resolution or tenant isolation layer.

---

# 5. DEPENDENCY RULE

Dependencies must always point inward.

The primary dependency flow is:

```text
Presentation
      ↓
Application
      ↓
Domain
```

Infrastructure implements abstractions defined by inner layers:

```text
Infrastructure
      ↓
Application / Domain
```

Domain must have zero dependency on:

* ASP.NET Core
* Razor Pages
* EF Core
* PostgreSQL
* Redis
* RabbitMQ
* HTTP
* JavaScript
* Infrastructure
* External APIs
* Third-party infrastructure libraries

Domain must remain framework-independent.

---

# 6. INTERFACE OWNERSHIP

Interfaces must belong to the layer that owns the abstraction.

Repository abstractions belong to Domain/Application according to their responsibility.

Implementations belong to Infrastructure.

Example:

```text
Domain/
└── Repositories/
    └── IUserRepository.cs

Infrastructure/
└── Repositories/
    └── UserRepository.cs
```

Application services depend on:

```csharp
IUserRepository
```

and never:

```csharp
UserRepository
```

---

# 7. SOLUTION STRUCTURE

Use a single ASP.NET Core Web project.

Recommended structure:

```text
Solution.sln

├── src/
│   └── App/
│       ├── BuildingBlocks/
│       │
│       ├── Modules/
│       │   └── {ModuleName}/
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
│       │   ├── css/
│       │   ├── js/
│       │   ├── images/
│       │   └── lib/
│       │
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.{Environment}.json
│
├── tests/
│   ├── App.UnitTests/
│   ├── App.IntegrationTests/
│   └── e2e/
│
├── docker-compose.yml
└── Dockerfile
```

There must be only one deployable application.

---

# 8. BUILDING BLOCKS

BuildingBlocks contains only genuinely shared infrastructure and abstractions.

Recommended:

```text
BuildingBlocks/
├── Domain/
│   ├── Abstractions/
│   ├── Primitives/
│   └── Exceptions/
│
├── Application/
│   ├── Abstractions/
│   └── Common/
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

Do NOT place business-specific logic inside BuildingBlocks.

BuildingBlocks must never reference individual modules.

---

# 9. MODULE STRUCTURE

Each module should follow:

```text
Modules/
└── UserManagement/
    ├── Domain/
    │   ├── Entities/
    │   ├── ValueObjects/
    │   ├── Events/
    │   ├── Exceptions/
    │   ├── Repositories/
    │   └── Enums/
    │
    ├── Application/
    │   ├── DTOs/
    │   ├── ViewModels/
    │   ├── Services/
    │   ├── Validators/
    │   ├── Pipelines/
    │   │   └── {Feature}/
    │   └── EventHandlers/
    │
    ├── Infrastructure/
    │   ├── Persistence/
    │   │   ├── UserManagementDbContext.cs
    │   │   ├── Configurations/
    │   │   └── Seeders/
    │   ├── Repositories/
    │   ├── Services/
    │   └── Messaging/
    │
    └── Presentation/
        ├── Pages/
        ├── Components/
        ├── Partials/
        └── ModuleRoute.cs
```

Each module must be internally cohesive.

---

# 10. MODULE REGISTRATION

Every module must expose a registration method:

```csharp
public static IServiceCollection Add{Module}Module(
    this IServiceCollection services,
    IConfiguration configuration)
```

Module registration must register:

* DbContext
* Repository implementations
* Application services
* Validators
* Pipeline handlers
* Domain event handlers
* Module-specific infrastructure
* Razor Page conventions where required

`Program.cs` remains the composition root.

Example:

```csharp
builder.Services.AddUserManagementModule(
    builder.Configuration);
```

---

# 11. PRESENTATION ARCHITECTURE

Use:

* ASP.NET Core Razor Pages
* Razor PageModels
* Razor Partial Views
* View Components
* Tag Helpers where appropriate
* ES6 JavaScript Modules
* Fetch API
* Server-side rendering
* AJAX enhancement where beneficial

Razor Pages are the primary UI architecture.

Do NOT use MVC Controllers for normal UI flows.

Do NOT create one controller per page.

Do NOT create controllers simply to support AJAX.

---

# 12. RAZOR PAGE STRUCTURE

A feature should look like:

```text
Presentation/
└── Pages/
    └── Users/
        ├── Index.cshtml
        ├── Index.cshtml.cs
        ├── Create.cshtml
        ├── Create.cshtml.cs
        ├── Edit.cshtml
        ├── Edit.cshtml.cs
        ├── Details.cshtml
        ├── Details.cshtml.cs
        └── users.js
```

Pages must be organized by:

```text
Module → Feature → Page
```

Never organize the application like:

```text
Pages/
├── AllPageModels/
├── AllViewModels/
├── AllJavaScript/
└── AllRazorFiles/
```

The physical structure must communicate module ownership.

---

# 13. PAGE MODEL RESPONSIBILITY

A PageModel is responsible only for:

* Receiving UI input
* Binding input models
* Calling Application services
* Mapping DTOs to ViewModels
* Preparing page state
* Returning Razor Page results
* Redirecting after successful operations
* Authorization
* Coordinating UI-specific behavior

PageModels must NOT contain:

* Business rules
* Database queries
* EF Core code
* DbContext access
* Repository implementations
* Redis access
* RabbitMQ access
* External API calls
* Complex workflows

Example:

```text
Razor Page
     ↓
PageModel
     ↓
IUserService
     ↓
IUserRepository
     ↓
UserRepository
     ↓
UserDbContext
```

Never:

```text
PageModel
     ↓
DbContext
```

---

# 14. RAZOR PAGE + APPLICATION SEPARATION

The dependency flow must be:

```text
Razor Page
     ↓
PageModel
     ↓
Application Service
     ↓
Domain
```

The Presentation layer must never directly depend on Infrastructure.

Presentation must NOT reference:

```text
EF Core
DbContext
Repository implementations
Redis
RabbitMQ
External APIs
Infrastructure services
```

---

# 15. MODULAR JAVASCRIPT

JavaScript must use native ES Modules.

Do NOT create one giant:

```text
site.js
app.js
main.js
```

containing all application behavior.

Use:

```text
wwwroot/
└── js/
    ├── core/
    │   ├── api-client.js
    │   ├── http.js
    │   ├── modal.js
    │   ├── notification.js
    │   ├── validation.js
    │   ├── table.js
    │   └── dom.js
    │
    └── modules/
        ├── user-management/
        │   ├── users/
        │   │   ├── index.js
        │   │   ├── create.js
        │   │   └── edit.js
        │   │
        │   └── roles/
        │       ├── index.js
        │       └── permissions.js
        │
        └── {other-module}/
```

Use:

```javascript
import { apiClient } from "../../core/api-client.js";
import { showNotification } from "../../core/notification.js";

export function initializeUsersPage() {
    // page-specific behavior
}
```

Avoid global variables:

```javascript
window.App = ...
window.Users = ...
window.UserManager = ...
```

unless an explicit browser integration requires them.

---

# 16. PAGE-SPECIFIC JAVASCRIPT

Each Razor Page should load only the JavaScript required by that page.

Example:

```html
@section Scripts {
    <script
        type="module"
        src="~/js/modules/user-management/users/index.js">
    </script>
}
```

Each module should have a clear initialization entry point.

Example:

```javascript
document.addEventListener("DOMContentLoaded", () => {
    initializeUsersPage();
});
```

Shared behavior belongs in:

```text
wwwroot/js/core/
```

Feature behavior belongs in:

```text
wwwroot/js/modules/{module}/{feature}/
```

Never duplicate:

* AJAX handling
* Notifications
* Modals
* HTTP handling
* Validation helpers
* Confirmation dialogs

across modules.

---

# 17. JAVASCRIPT HTTP ABSTRACTION

All asynchronous HTTP communication must go through:

```text
wwwroot/js/core/api-client.js
```

Do not scatter raw:

```javascript
fetch(...)
```

through feature modules.

The shared API client is responsible for:

* GET
* POST
* PUT
* DELETE
* JSON serialization
* JSON deserialization
* Antiforgery handling
* HTTP error handling
* Authentication/session handling where applicable
* Standardized response processing

Conceptual API:

```javascript
apiClient.get(url);
apiClient.post(url, data);
apiClient.put(url, data);
apiClient.delete(url);
```

Feature JavaScript must depend on the abstraction rather than browser networking details.

---

# 18. JAVASCRIPT RESPONSIBILITY

JavaScript is responsible for:

* UI interaction
* Dynamic controls
* AJAX requests
* Client-side enhancement
* Modal dialogs
* Dynamic tables
* Filtering
* Sorting
* Partial UI refresh
* Notifications

JavaScript must NOT implement business rules.

Mandatory rule:

> JavaScript controls UI behavior; Application services control use cases; Domain controls business invariants.

---

# 19. FORMS

Prefer normal Razor Page form submission for simple operations.

Example:

```text
POST /Users/Create
```

For highly interactive operations, use Razor Page named handlers.

Examples:

```text
POST /Users?handler=Delete
POST /Users?handler=ToggleStatus
```

PageModel:

```csharp
public async Task<IActionResult> OnPostAsync()

public async Task<IActionResult> OnPostDeleteAsync()

public async Task<IActionResult> OnPostToggleStatusAsync()
```

Do not introduce controllers simply to support these operations.

If a reusable API is genuinely required by multiple clients, it may be exposed as an API endpoint, but this must be an exception.

---

# 20. SECURITY

All state-changing Razor Page requests must use ASP.NET Core antiforgery protection.

This includes:

* POST
* PUT
* DELETE
* AJAX mutations

Never disable antiforgery validation for convenience.

Authentication and authorization must always be enforced server-side.

Never trust:

* Hidden fields
* JavaScript state
* Query strings
* Client-side permission checks

The server is always authoritative.

---

# 21. USER MANAGEMENT MODULE

User Management is a dedicated module.

It owns:

* Users
* Roles
* Permissions
* Authentication
* Authorization integration
* Password management
* Refresh tokens if required
* Account status
* Account lifecycle

It does NOT own:

* Tenants
* Tenant status
* Tenant provisioning
* Tenant isolation
* Tenant selection

Recommended structure:

```text
Modules/UserManagement/

├── Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   └── RefreshToken.cs
│   ├── ValueObjects/
│   │   ├── Email.cs
│   │   └── PasswordHash.cs
│   ├── Events/
│   ├── Exceptions/
│   ├── Repositories/
│   └── Enums/
│
├── Application/
│   ├── DTOs/
│   ├── ViewModels/
│   ├── Services/
│   ├── Validators/
│   └── Pipelines/
│
├── Infrastructure/
│   ├── Persistence/
│   ├── Repositories/
│   └── Services/
│
└── Presentation/
    └── Pages/
        ├── Account/
        │   ├── Login.cshtml
        │   ├── Login.cshtml.cs
        │   ├── Register.cshtml
        │   └── Register.cshtml.cs
        │
        ├── Users/
        │   ├── Index.cshtml
        │   ├── Index.cshtml.cs
        │   ├── Create.cshtml
        │   ├── Create.cshtml.cs
        │   ├── Edit.cshtml
        │   └── Edit.cshtml.cs
        │
        └── Roles/
            ├── Index.cshtml
            ├── Index.cshtml.cs
            └── Edit.cshtml.cs
```

---

# 22. AUTHENTICATION

Because Razor Pages are server-rendered, use:

```text
ASP.NET Core Cookie Authentication
```

for browser authentication.

Use:

* Secure cookies
* HttpOnly cookies
* Appropriate SameSite configuration
* CSRF protection
* Permission-based authorization

JWT may be supported only when genuinely required for:

* External APIs
* Machine-to-machine communication
* Other non-browser clients

Do not force JWT into the Razor Page UI.

The browser UI should not store access tokens in:

```text
localStorage
sessionStorage
JavaScript variables
```

unless there is an explicit architectural requirement.

---

# 23. AUTHENTICATION PIPELINE

Authentication workflows with multiple sequential steps must use Chain of Responsibility.

Example:

```text
CheckAccountLockout
        ↓
ValidateCredentials
        ↓
CheckAccountStatus
        ↓
CheckMfaIfEnabled
        ↓
GenerateAuthenticationResult
```

Each handler must have one responsibility.

Do not create one huge authentication method containing many conditions.

---

# 24. AUTHORIZATION

Authorization must be permission-based.

Avoid:

```csharp
if (user.Role == "Admin")
{
    ...
}
```

inside Razor Pages.

Use:

* ASP.NET Core policies
* Permission-based authorization
* Custom authorization handlers where appropriate
* Custom permission attributes where appropriate

Centralize permission definitions:

```text
BuildingBlocks/
└── Infrastructure/
    └── Authorization/
        ├── PermissionCatalog.cs
        ├── PermissionPolicyProvider.cs
        └── RequirePermissionAttribute.cs
```

The UI may hide unauthorized controls for usability.

However, the server must always enforce authorization.

---

# 25. DOMAIN-DRIVEN DESIGN

Use DDD tactical patterns where they provide actual value.

Use:

* Entities
* Aggregates
* Value Objects
* Domain Events
* Domain Exceptions
* Repository abstractions

Domain entities must protect their own invariants.

Do not move business rules into:

* Razor Pages
* PageModels
* JavaScript
* Infrastructure
* Controllers

---

# 26. VALIDATION

Use FluentValidation.

There should generally be one validator per input DTO.

Validation must happen server-side before application processing.

Client-side validation may improve UX but must never replace server-side validation.

Avoid large manual validation blocks inside PageModels.

---

# 27. VIEW MODELS

Never expose Domain entities directly to Razor Pages.

Use dedicated ViewModels.

Example:

```text
Application/
├── DTOs/
│   ├── CreateUserRequest.cs
│   └── UpdateUserRequest.cs
│
└── ViewModels/
    ├── UserListViewModel.cs
    ├── UserDetailsViewModel.cs
    └── UserEditViewModel.cs
```

Flow:

```text
Razor Page
     ↓
ViewModel
     ↓
Application DTO
     ↓
Application Service
     ↓
Domain
```

Never:

```text
Razor Page
     ↓
EF Entity
```

---

# 28. PARTIAL VIEWS

Use Partial Views for reusable UI components.

Examples:

```text
Partials/
├── _UserTable.cshtml
├── _UserForm.cshtml
├── _Pagination.cshtml
├── _ConfirmDialog.cshtml
└── _StatusBadge.cshtml
```

Partials must remain presentation-only.

Never place business logic in Partial Views.

---

# 29. VIEW COMPONENTS

Use View Components when reusable UI requires server-side application data.

Examples:

```text
Components/
├── NavigationMenu/
├── UserMenu/
└── NotificationSummary/
```

A View Component may call an Application service.

It must never directly access:

* DbContext
* Repository implementation
* Infrastructure implementation

There must be no TenantSelector View Component.

---

# 30. LAYOUT

Use:

```text
Pages/
└── Shared/
    ├── _Layout.cshtml
    ├── _ValidationScriptsPartial.cshtml
    ├── _Notification.cshtml
    └── _Error.cshtml
```

The layout provides:

* Navigation
* Authentication-aware UI
* Common CSS
* Core JavaScript
* Notification container
* Modal container
* Antiforgery infrastructure
* Accessibility behavior

Feature JavaScript must remain feature-specific.

---

# 31. DATABASE ARCHITECTURE

Use:

```text
PostgreSQL
+
EF Core
+
Npgsql
```

Use one DbContext per module.

Example:

```text
UserManagementDbContext
NotificationDbContext
FileManagementDbContext
```

The architecture must use:

* Repository Pattern
* Centralized migrations
* Soft delete where appropriate
* Auditing
* UTC timestamps
* Explicit snake_case database naming
* Appropriate indexes

There is no tenant filtering.

There are no tenant indexes.

There are no tenant query filters.

Razor Pages must have absolutely no knowledge of EF Core.

---

# 32. REPOSITORY PATTERN

Use Repository Pattern instead of CQRS.

Repositories must:

* Be defined behind interfaces
* Be implemented in Infrastructure
* Encapsulate query construction
* Never expose IQueryable
* Never expose DbSet
* Never expose EF-specific types
* Explicitly control eager loading
* Avoid lazy loading
* Avoid N+1 queries

Example:

```text
IUserRepository
      ↓
UserRepository
      ↓
UserManagementDbContext
```

Application services depend on:

```csharp
IUserRepository
```

never:

```csharp
UserRepository
```

---

# 33. APPLICATION SERVICES

Use Application Services for normal use cases.

Example:

```text
IUserService
      ↓
UserService
```

Application Services are responsible for:

* Use cases
* Application orchestration
* DTO processing
* Validation coordination
* Repository coordination
* Domain interaction
* Transaction boundaries
* Event publishing coordination

Do NOT introduce CQRS/MediatR.

Do not create command/query handlers merely to make the architecture appear sophisticated.

---

# 34. CHAIN OF RESPONSIBILITY

Use Chain of Responsibility when a workflow contains multiple sequential processing steps.

Example:

```text
UserService
      ↓
RegistrationPipeline
      ↓
ValidateInput
      ↓
CheckDuplicateUser
      ↓
CreateUser
      ↓
RaiseUserCreatedEvent
```

Use it to prevent:

* Long methods
* Deeply nested conditions
* Large if/else blocks
* Difficult-to-test workflows

Each handler should have one responsibility.

Do not use Chain of Responsibility for simple workflows where it adds unnecessary complexity.

---

# 35. STRATEGY PATTERN

Use Strategy Pattern when multiple interchangeable implementations exist.

Examples:

```text
IFileStorageStrategy
IEmailProviderStrategy
INotificationStrategy
```

Example:

```text
NotificationService
        ↓
INotificationStrategy
        ├── EmailNotificationStrategy
        ├── SmsNotificationStrategy
        └── PushNotificationStrategy
```

Do not use long conditional blocks such as:

```csharp
if (type == "Email")
{
}
else if (type == "SMS")
{
}
else if (type == "Push")
{
}
```

when the behavior represents interchangeable strategies.

---

# 36. OTHER DESIGN PATTERNS

Use patterns only when they solve a real architectural problem.

Supported patterns include:

### Repository

Persistence abstraction.

### Unit of Work

Implicit through the module DbContext and one SaveChanges operation per use case.

### Chain of Responsibility

Multi-step workflows.

### Strategy

Interchangeable behavior/provider implementations.

### Factory

Complex aggregate construction.

### Domain Events

Decoupled module communication.

### Decorator / Interceptor

Cross-cutting persistence behavior such as:

* Auditing
* Soft delete

### Cache-Aside

Redis caching.

Do not introduce patterns merely because they are available.

---

# 37. MODULE COMMUNICATION

Modules must remain isolated.

A module must never directly reference another module's namespace.

Avoid:

```text
UserManagement → Notification
```

through direct implementation references.

Use:

* Domain Events
* Shared abstractions
* Application contracts where genuinely required

Modules must never call each other over HTTP.

Modules must never communicate through controllers.

Modules must never directly access another module's:

* DbContext
* Repository
* Domain entity
* Infrastructure service
* Internal JavaScript

---

# 38. DOMAIN EVENTS

Use Domain Events when a module needs to communicate that something happened.

Example:

```text
UserManagement
      ↓
UserCreated
      ↓
Notification
```

The publishing module should not directly invoke the Notification module's implementation.

Events should provide loose coupling.

---

# 39. CROSS-CUTTING CONCERNS

Cross-cutting concerns must be implemented centrally.

| Concern            | Mechanism                    |
| ------------------ | ---------------------------- |
| Correlation ID     | Middleware                   |
| Exception handling | Middleware                   |
| Authentication     | ASP.NET Core Authentication  |
| Authorization      | ASP.NET Core Policies        |
| Validation         | FluentValidation             |
| Auditing           | EF interceptor               |
| Soft delete        | EF interceptor/query filters |
| Caching            | Redis abstraction            |
| Rate limiting      | ASP.NET Core rate limiting   |
| Logging            | Serilog                      |
| Metrics            | OpenTelemetry/Prometheus     |
| Antiforgery        | ASP.NET Core antiforgery     |

Do not duplicate cross-cutting concerns inside modules.

There is no tenant resolution mechanism.

There is no tenant status mechanism.

---

# 40. ERROR HANDLING

Use centralized exception handling.

Flow:

```text
Exception
    ↓
Global Exception Middleware
    ↓
Structured Logging
    ↓
User-Friendly Error Response/Page
```

Do not catch exceptions inside every PageModel merely to convert them into UI errors.

Expected validation/domain failures should be translated into appropriate:

* Validation messages
* Notifications
* Page-level errors

Unexpected exceptions must never expose:

* Stack traces
* SQL errors
* Connection strings
* Internal infrastructure details

---

# 41. CACHING

Use Redis Cache-Aside behind:

```csharp
ICacheService
```

Application code must not directly depend on Redis implementation details.

JavaScript must never communicate with Redis.

Centralize cache keys:

```text
CacheKeys
```

Centralize expiration categories:

```text
CacheExpiration
```

Do not create arbitrary cache keys throughout the application.

Cache keys must not contain TenantId or tenant information.

---

# 42. MESSAGING

RabbitMQ may be used for asynchronous processing.

Messaging remains Infrastructure.

Razor Pages must never know about:

```text
RabbitMQ
Exchange
Queue
RoutingKey
```

A PageModel only communicates with an Application service.

Application services communicate through appropriate abstractions.

---

# 43. OBSERVABILITY

Use:

* Serilog
* Structured logging
* Correlation IDs
* OpenTelemetry
* Metrics

Every request should have a correlation identifier.

The correlation identifier must be available to:

* PageModels
* Application services
* Infrastructure services
* Logs

without directly accessing HttpContext from business/domain code.

---

# 44. RATE LIMITING

Use ASP.NET Core rate limiting where required.

Rate limiting must be configured centrally.

Do not implement custom rate limiting logic separately inside every module.

---

# 45. TESTING ARCHITECTURE

Use three testing levels.

## Unit Tests

Test:

* Domain
* Application Services
* Pipeline handlers
* Strategies
* Validators
* PageModels where useful

## Integration Tests

Use:

* Testcontainers
* Real PostgreSQL
* WebApplicationFactory
* Real EF Core pipeline

Test:

* Razor Page requests
* Authentication
* Authorization
* Validation
* Database behavior
* Repository behavior
* Application service behavior
* Cross-module event behavior

Do not create tenant isolation tests because this is a single-tenant application.

## End-to-End Tests

Use Playwright.

Test real browser flows:

```text
Login
  ↓
Navigate to page
  ↓
Fill form
  ↓
Submit
  ↓
Server processing
  ↓
UI update
```

E2E tests must cover:

* Normal Razor Page navigation
* JavaScript-enhanced interactions
* Authentication
* Authorization
* Form validation
* CRUD workflows

---

# 46. CODING CONVENTIONS

Use:

* Nullable reference types
* `sealed` by default
* `private set`
* `private init`
* Async all the way down
* CancellationToken
* One class per file
* Namespaces matching folders
* No magic strings
* No magic numbers
* Explicit database naming
* UTC timestamps

Razor conventions:

* One PageModel per Razor Page
* No business logic in `.cshtml`
* No database access in `.cshtml`
* No Infrastructure dependency in PageModels
* Strongly typed Razor models
* Reusable UI through Partial Views/View Components
* Page-specific JavaScript loaded explicitly
* ES Modules instead of global JavaScript
* Accessible HTML
* Server-side validation always enabled

---

# 47. PROJECT DEPENDENCY RULES

The following rules are mandatory.

1. BuildingBlocks may be referenced by modules.
2. BuildingBlocks may never reference modules.
3. One module may never reference another module's namespace directly.
4. Domain references nothing external.
5. Application references its own Domain and permitted BuildingBlocks abstractions.
6. Infrastructure references Domain, Application, and infrastructure libraries.
7. Presentation references only Application and permitted BuildingBlocks presentation abstractions.
8. Presentation never references Infrastructure.
9. Razor Pages never access DbContext.
10. Razor Pages never access repositories directly.
11. JavaScript never contains business logic.
12. JavaScript never communicates directly with Infrastructure.
13. Cross-module communication occurs through domain events or explicitly defined shared abstractions.
14. Modules never call each other through HTTP.
15. No module imports another module's JavaScript.
16. Shared JavaScript belongs only under `wwwroot/js/core`.
17. Module JavaScript belongs under that module's namespace.
18. No global mutable JavaScript state.
19. No TenantId anywhere unless explicitly introduced as a future business requirement.
20. No multi-tenancy infrastructure.
21. No tenant resolution.
22. No tenant filtering.
23. No tenant-specific authorization.
24. No tenant-specific caching.

---

# 48. EXPECTED FINAL FOLDER STRUCTURE

The final architecture should resemble:

```text
src/App/

├── BuildingBlocks/
│   ├── Domain/
│   │   ├── Abstractions/
│   │   ├── Primitives/
│   │   └── Exceptions/
│   │
│   ├── Application/
│   │   ├── Abstractions/
│   │   └── Common/
│   │
│   └── Infrastructure/
│       ├── Authentication/
│       ├── Authorization/
│       ├── Caching/
│       ├── Events/
│       ├── Filters/
│       ├── Messaging/
│       ├── Middleware/
│       ├── Observability/
│       ├── Options/
│       ├── Persistence/
│       ├── RateLimiting/
│       ├── Security/
│       ├── Storage/
│       └── Services/
│
├── Modules/
│   └── UserManagement/
│       ├── Domain/
│       │   ├── Entities/
│       │   ├── ValueObjects/
│       │   ├── Events/
│       │   ├── Exceptions/
│       │   ├── Repositories/
│       │   └── Enums/
│       │
│       ├── Application/
│       │   ├── DTOs/
│       │   ├── ViewModels/
│       │   ├── Services/
│       │   ├── Validators/
│       │   ├── Pipelines/
│       │   └── EventHandlers/
│       │
│       ├── Infrastructure/
│       │   ├── Persistence/
│       │   ├── Repositories/
│       │   ├── Services/
│       │   └── Messaging/
│       │
│       └── Presentation/
│           ├── Pages/
│           │   ├── Account/
│           │   ├── Users/
│           │   └── Roles/
│           ├── Components/
│           ├── Partials/
│           └── ModuleRoute.cs
│
├── Migrations/
│
├── Pages/
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _ValidationScriptsPartial.cshtml
│       ├── _Notification.cshtml
│       └── _Error.cshtml
│
├── wwwroot/
│   ├── css/
│   ├── images/
│   ├── lib/
│   └── js/
│       ├── core/
│       │   ├── api-client.js
│       │   ├── http.js
│       │   ├── modal.js
│       │   ├── notification.js
│       │   ├── validation.js
│       │   ├── table.js
│       │   └── dom.js
│       │
│       └── modules/
│           └── user-management/
│               ├── users/
│               │   ├── index.js
│               │   ├── create.js
│               │   └── edit.js
│               └── roles/
│                   ├── index.js
│                   └── permissions.js
│
├── Program.cs
├── appsettings.json
└── appsettings.{Environment}.json
```

---

# 49. ARCHITECTURAL CONSTRAINTS

The implementation MUST satisfy all of the following:

* Single deployable ASP.NET Core application
* Modular Monolith
* Strict module boundaries
* Vertical slice architecture
* DDD tactical patterns
* Razor Pages as primary presentation layer
* Modular ES6 JavaScript
* No SPA framework
* No MVC Controllers for normal UI flows
* Repository Pattern
* No CQRS
* No MediatR
* Chain of Responsibility for multi-step workflows
* Strategy Pattern where interchangeable implementations are required
* PostgreSQL
* EF Core
* Npgsql
* One DbContext per module
* Redis Cache-Aside
* RabbitMQ for asynchronous processing where required
* Permission-based authorization
* Cookie authentication for Razor UI
* Optional JWT only when external APIs genuinely require it
* Global validation
* Global exception handling
* Global auditing
* Global soft delete where appropriate
* Correlation IDs
* Structured logging
* OpenTelemetry
* Testcontainers
* Playwright E2E testing
* Strict Infrastructure isolation
* Strict Presentation isolation
* No direct cross-module implementation dependencies
* No business logic in Razor Pages
* No business logic in JavaScript
* No multi-tenancy

---

# 50. IMPLEMENTATION RULES FOR AI CODING AGENTS

When implementing this architecture:

## Step 1 — Inspect

First inspect the existing project thoroughly.

Understand:

* Existing solution structure
* Existing modules
* Existing business functionality
* Existing dependencies
* Existing database structure
* Existing authentication
* Existing infrastructure
* Existing tests

Do not immediately rewrite the project.

## Step 2 — Architecture Gap Analysis

Compare the existing implementation against this architecture specification.

Identify:

* Architectural violations
* Missing boundaries
* Incorrect dependencies
* Controller usage that should become Razor Pages
* Business logic inside Presentation
* Infrastructure leakage
* Repository violations
* Missing Application services
* Missing Domain abstractions
* Large conditional workflows
* Opportunities for Strategy Pattern
* Opportunities for Chain of Responsibility
* Missing cross-cutting abstractions
* Unnecessary coupling

## Step 3 — Implementation Plan

Before making major changes, produce a clear implementation plan.

The plan should identify:

```text
Current Architecture
        ↓
Problems
        ↓
Target Architecture
        ↓
Migration Steps
        ↓
Risks
        ↓
Testing Strategy
```

## Step 4 — Incremental Implementation

Implement incrementally.

Do not perform an uncontrolled rewrite.

After each significant architectural change:

1. Build the solution.
2. Run relevant unit tests.
3. Run relevant integration tests.
4. Fix compilation errors.
5. Fix test failures.
6. Verify dependency boundaries.
7. Continue to the next change.

## Step 5 — Preserve Business Behavior

The architecture may change.

The existing business behavior must not be changed unless explicitly requested.

The goal is:

```text
Existing Business Logic
        +
Improved Architecture
        =
Target Application
```

Do not invent new business requirements.

---

# 51. WHAT NOT TO DO

Never introduce the following unless explicitly requested:

```text
Microservices
CQRS
MediatR
Event Sourcing
Multi-Tenancy
TenantId
TenantContext
GraphQL
React
Angular
Vue
Blazor
SPA architecture
Controller-per-page architecture
Repository implementations in Presentation
DbContext in PageModels
Business logic in JavaScript
Business logic in Razor Views
Direct Redis usage from UI
Direct RabbitMQ usage from UI
HTTP communication between modules
Global mutable JavaScript state
```

Do not over-engineer simple features.

Architecture should solve real problems rather than introduce unnecessary abstractions.

---

# 52. FINAL ARCHITECTURAL PRINCIPLE

The final application should have the following conceptual structure:

```text
                         ASP.NET CORE APPLICATION
                                  │
              ┌───────────────────┴───────────────────┐
              │                                       │
         Razor Pages                           Modular ES6 JS
              │                                       │
              ▼                                       │
      ┌───────────────┐                               │
      │  Presentation │                               │
      └───────┬───────┘                               │
              │                                       │
              ▼                                       │
      ┌───────────────┐                               │
      │  Application  │◄──────────────────────────────┘
      │   Services    │
      └───────┬───────┘
              │
              ▼
      ┌───────────────┐
      │    Domain     │
      │               │
      │ Entities      │
      │ Aggregates    │
      │ Value Objects │
      │ Domain Events │
      └───────▲───────┘
              │
              │
      ┌───────┴────────┐
      │ Infrastructure │
      │                │
      │ EF Core        │
      │ PostgreSQL     │
      │ Redis          │
      │ RabbitMQ       │
      │ Authentication │
      │ Storage        │
      │ Observability  │
      └────────────────┘
```

The architecture must remain:

```text
MODULAR MONOLITH
        +
STRICT MODULE BOUNDARIES
        +
VERTICAL SLICES
        +
DDD
        +
REPOSITORY PATTERN
        +
CHAIN OF RESPONSIBILITY
        +
STRATEGY PATTERN
        +
RAZOR PAGES
        +
MODULAR ES6 JAVASCRIPT
        +
POSTGRESQL / EF CORE
        +
REDIS
        +
RABBITMQ
        +
ENTERPRISE SECURITY
        +
OBSERVABILITY
        +
AUTOMATED TESTING
```

And explicitly:

```text
NO MULTI-TENANCY
NO CQRS
NO MEDIATR
NO SPA
NO CONTROLLER-FIRST UI
NO BUSINESS LOGIC IN UI
NO CROSS-MODULE IMPLEMENTATION COUPLING
```

Most importantly:

The resulting application should feel like a scalable enterprise application architecturally while remaining maintainable, modular, testable, and free from unnecessary complexity.
