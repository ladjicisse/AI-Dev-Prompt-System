# LADCI.AI.Invoicing

DDD scaffold for the Invoicing application targeting `.NET 10`.

Architecture rules:
- `Domain` has zero external dependencies.
- `Application` orchestrates use cases only.
- `Infrastructure` handles EF Core, external APIs, and service integrations.
- `API` is only a delivery mechanism.

Dependency direction:
- `API -> Application`
- `API -> Infrastructure`
- `Infrastructure -> Application`
- `Infrastructure -> Domain`
- `Application -> Domain`

The build includes guardrails in `Directory.Build.targets` to help enforce these boundaries as the codebase grows.
