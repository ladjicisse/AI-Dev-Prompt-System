# Infrastructure

This layer implements technical concerns and external integrations.

Rules:
- Put EF Core, persistence mappings, repositories, external APIs, messaging, and service clients here.
- It may depend on `Application` and `Domain`.
- Keep business decisions out of this layer.

Suggested structure:
- `DependencyInjection/`
- `Persistence/`
