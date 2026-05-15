# AI Development Prompt System

How do you turn AI into a disciplined DDD + TDD collaborator rather than a code generator?

Here’s my most effective approach in practice.

### 1. Start with DDD — but make it AI-readable
   
AI struggles when the domain is vague. Classic `DDD` artifacts become even more critical—but they must be explicit and structured.

What you should produce first (before any code prompt)
- Ubiquitous Language glossary
- Bounded Context definitions
- Aggregates + invariants
- Domain events
- Use cases (behavior-focused, not technical)

### 2. Use TDD as the control system for AI

Always prompt AI in this sequence:
1. Write failing tests
2. Validate test intent
3. Implement minimal code to pass
4. Refactor

_Example prompt pattern_

```text
Given this domain model, generate unit tests for the Invoice aggregate enforcing invariants and domain events. Do not implement the class.
```

### 3. Treat AI like a junior developer (strict boundaries)

AI needs constraints, otherwise it:
- Mixes layers (domain + infra)
- Leaks persistence into domain
- Breaks aggregate rules

**Define architectural rules explicitly**

```text
- Domain layer must not depend on infrastructure
- Aggregates enforce all invariants
- Repositories are interfaces in domain
- EF Core only in infrastructure layer
```

### 4. Use “prompt templates” as your new engineering asset

Your leverage is not just code—it’s repeatable prompts.

Core templates you should build

**A. Domain modeling prompt**
“From this business requirement, extract aggregates, entities, value objects, and domain events. Respect DDD tactical patterns.”

**B. Test generation prompt**
“Generate unit tests for `[aggregate]` covering invariants and edge cases.”

**C. Implementation prompt**
“Implement code that passes these tests. Follow clean architecture and DDD constraints.”

**D. Refactoring prompt**
“Refactor for readability and separation of concerns without changing behavior.”

### 5. Introduce “AI checkpoints” in your workflow

Instead of trusting a single generation, add validation loops.

Example workflow
1. Domain modeling (AI + human validation)
2. Test generation (AI)
3. Human review of test coverage
4. Implementation (AI)
5. Static analysis + code review
6. Refactoring (AI-assisted)

### 6. Keep humans responsible for the hard parts

AI is weak at:
- Strategic DDD (bounded context boundaries)
- Trade-offs (performance vs purity)
- Long-term architecture evolution

So you should own:
- Context mapping
- Domain decomposition
- Critical invariants
- Security decisions

### 7. Use AI to accelerate—not replace—design thinking

**Bad approach:**

“Build me a billing system with DDD”

**Good approach:**

“Here is my domain model. Enforce it strictly and generate tests first.”


### 8. Tooling stack (pragmatic setup)

Given your .NET + DevOps background, a strong setup would be:
- GitHub Copilot or Cursor for inline generation
- ChatGPT for structured prompts and domain modeling
- .NET with:
- xUnit / NUnit for TDD
- FluentAssertions
- CI enforcing:
- test coverage
- linting
- architectural rules (NetArchTest)

### 9. The mental model shift

**Before AI:**

Code → Tests → Fix

**With AI:**

Domain → Tests → AI Code → Validate → Refine


### 10. A concrete starter workflow for your next project
1.	Write domain YAML (like above)
2.	Prompt AI to extract aggregates + rules
3.	Generate test suite per aggregate
4.	Review tests manually
5.	Generate implementation
6.	Run CI pipeline
7.	Iterate with refactoring prompts

---

Bottom line

AI doesn’t remove the need for DDD and TDD—it makes them more important.

If you don’t impose:
	•	structure (DDD)
	•	discipline (TDD)

you’ll get fast code… that collapses under real-world complexity.