# Domain

This layer contains the core business model for invoicing.

Rules:
- Zero external dependencies.
- No references to `Application`, `Infrastructure`, or `API`.
- Keep aggregates, entities, value objects, domain services, and domain events here.

Suggested structure:
- `Aggregates/`
- `Entities/`
- `Events/`
- `ValueObjects/`
