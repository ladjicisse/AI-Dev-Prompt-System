# API

This layer is the delivery mechanism for the application.

Rules:
- Keep controllers and contracts thin.
- Delegate use case execution to `Application`.
- Use `Infrastructure` only for composition root and service registration.
- Do not place domain rules or EF Core persistence logic here.

Suggested structure:
- `Contracts/`
- `Controllers/`
