# Application

This layer orchestrates use cases and coordinates the domain model.

Rules:
- Depend on `Domain` only.
- No EF Core, HTTP clients, or infrastructure concerns.
- Define commands, queries, handlers, ports, and application services here.

Suggested structure:
- `Abstractions/`
- `Invoices/Commands/`
- `Invoices/Queries/`
