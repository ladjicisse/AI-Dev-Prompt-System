# Complete reusable prompt system 
---
### Project template (Clean Architecture + DDD + TDD for .NET)

What you need is not just “prompts”, but a full AI-augmented development system combining:
	•	DDD structure
	•	TDD workflow
	•	Prompt orchestration
	•	DevOps enforcement

---
Steps to follow:

### 1. Target Architecture (baseline for all projects)

Every project must be like as follow:
```text

Generate project structure using .NET [VERSION] for the [NAME] application.

DDD Project structure:
src/
 ├── Domain/
 ├── Application/
 ├── Infrastructure/
 └── API/

 ├── Dockerfile
 ├── .gitignore.yml
 ├── .gitlab-ci.yml

tests/
 ├── UnitTests/
 └── IntegrationTests/

Project Namespace:
[NAMESPACE]

Project folder path: 
[PATH]

Rules (AI Constraints)
- Domain has ZERO external dependencies
- Application orchestrates use cases only
- Infrastructure handles EF Core, APIs, external services
- API is just a delivery mechanism
```

### 2. Your AI Prompt System (the real leverage)

Prompts to use in every projects.

**Prompt 1 -Domain extraction (DDD)**
```text
You are a senior DDD architect.

From the following business requirement:
[PASTE REQUIREMENT]

Extract:
- Bounded Context
- Aggregates
- Entities
- Value Objects
- Domain Events
- Invariants

Output in structured YAML.
Do NOT generate code.
```
**Prompt 2 — Aggregate Test Design (TDD entry point)**

```text
Given this domain model:
[PASTE YAML]

Generate unit tests for the Aggregate: [NAME]

Requirements:
- Cover all invariants
- Cover edge cases
- Cover domain events
- Use xUnit and FluentAssertions
- Do NOT implement the domain class
```

**Prompt 3 — Minimal Implementation**

```text
Implement the domain model to satisfy these tests.

Constraints:
- Follow DDD principles
- No EF Core or infrastructure concerns
- Enforce invariants inside the aggregate
- Keep code minimal

Output only code.
```

**Prompt 4 — Refactoring**

```text
Refactor this code:
[PASTE CODE]

Goals:
- Improve readability
- Enforce separation of concerns
- Maintain behavior (tests must still pass)
```

**Prompt 5 — Application Layer**

Use cases:
```text
Create an Application Service for:
[USE CASE]

Constraints:
- Use DTOs
- Call domain aggregates
- No business logic here
```
Unit tests for use cases:
```text

Generate unit tests for the application use cases:
[USE CASE]

Requirements:
- Use xUnit and FluentAssertions
- Break down each Command and QuerY test class in separate file

```

**Prompt 6 — Infrastructure (EF Core)**

```text
Implement EF Core configuration for:
[AGGREGATE]

Constraints:
- Respect aggregate boundaries
- Use Fluent API
- No business logic
```

**Prompt 7 - API (Endpoints)**

```text
Generate all API (endpoints) for use case:
[USE CASE]

Constraints:
- Use commands and Queries in application
- All enums must be expose as string (json string converter)
- OpenAPI must be available
- Add swagger UI

```

**Prompt 8 - Integration Tests**
```text

Generate integration tests for the API controller:
[CONTROLLER NAME]

Requirements:
- Use xUnit for the test framework.
- Use FluentAssertions for assertions.
- Create separate test files per use case:
  - one file for each command endpoint test class
  - one file for each query endpoint test class
- Organize tests by controller behavior, not by helper code.
- Cover:
  - successful requests
  - validation failures
  - not found cases where applicable
  - relevant error responses
- Keep test names explicit and behavior-focused.
- Reuse shared test infrastructure/helpers only when needed to avoid duplication.
- Do not mix command and query tests in the same class or file.
```

**Prompt 9 - Test coverage**
```text
Fix all tests and 
Run test coverage for the entire application
```

**Prompt 10 - Launch the app**

```text
Now, launch the app
```


### 4. Example End-to-End Flow (real scenario)
4. GitLab CI/CD Pipeline (DevOps enforcement)
5. Your “AI Guardrails” (critical)
6. Optional: Add Specification Pattern (advanced)