# Backend Architecture

## Goal

CodeCafe Next backend is a modular monolith for an AI Native workspace platform.

It is not a notes-only backend. Notes is only the first module. The architecture must support future modules such as code understanding, AI workflows, search, files, and collaboration without rewriting the backend structure.

## Core Principles

- Use a modular monolith, not microservices.
- Treat AI as a first-class module, not a thin chat endpoint.
- Prefer strong module boundaries over generic shared abstractions.
- Keep adapters thin and keep business rules in application flows.
- Build for long-term growth from the first implementation.

## Top-Level Structure

```text
src/
  BuildingBlocks/
    CodeCafe.SharedKernel/
    CodeCafe.BuildingBlocks/
  Modules/
    Platform/
    Notes/
    Code/
    AI/
  Adapters/
    Web/
    Mcp/
    Realtime/
  Host/
```

## Module Structure

Each module should follow this shape:

```text
CodeCafe.Modules.<Module>.Domain
CodeCafe.Modules.<Module>.Application
CodeCafe.Modules.<Module>.Infrastructure
CodeCafe.Modules.<Module>.Contracts
```

Responsibilities:

- `Domain`: entities, value objects, invariants, domain services, domain events
- `Application`: commands, queries, handlers, validators, interfaces, orchestration
- `Infrastructure`: EF Core, SDK integrations, persistence, background implementations
- `Contracts`: DTOs and cross-module integration contracts

## Planned Modules

### Platform

Owns cross-cutting platform capabilities:

- identity
- authorization
- audit
- sessions
- files
- search
- workspace-level concerns

Platform is not a dumping ground for random helpers. Only shared product capabilities belong here.

### Notes

Owns notebook, page, and note-related application behavior.

### Code

Owns repository reading, code indexing, code understanding, and future code-workspace features.

### AI

Owns:

- conversations
- tasks
- tool execution
- orchestration
- action planning
- integration with Microsoft Agent Framework

AI must interact with the system through application workflows and tool boundaries. It must not directly mutate storage.

## Adapter Structure

### Web

`CodeCafe.Web` is the HTTP adapter.

Owns:

- controllers or minimal APIs
- request/response mapping
- HTTP error mapping
- auth integration points specific to HTTP

Must not own:

- business rules
- direct DbContext usage for use-case logic
- cross-module orchestration that belongs in application code

### Mcp

`CodeCafe.Mcp` is the MCP adapter.

Owns:

- MCP tools
- MCP result shaping
- MCP-specific transport concerns

Must call the same application behavior as HTTP when serving equivalent use cases.

### Realtime

`CodeCafe.Realtime` is the SignalR adapter.

Owns:

- hubs
- realtime connection mapping
- push delivery contracts

Use realtime for:

- AI streaming responses
- task progress
- long-running workflow updates
- collaboration and presence later

Standard CRUD remains HTTP-first unless realtime clearly improves the workflow.

## Host

`CodeCafe.Host` is the only backend entry point.

Owns:

- application startup
- dependency injection composition
- adapter registration
- environment configuration
- middleware pipeline
- host-level policy

Business logic should not live here.

## Dependency Direction

Primary rule:

```text
Domain <- Application <- Infrastructure
                     <- Adapters
Host -> Adapters + Modules
```

Detailed rules:

- `Domain` references nothing outside itself except minimal shared primitives.
- `Application` may depend on `Domain` and stable shared abstractions.
- `Infrastructure` implements application interfaces.
- Adapters call application entry points.
- `Host` composes everything together.
- Modules should talk through `Contracts` or explicit application interfaces.
- A module must not depend on another module's infrastructure.

## Module Boundaries

Modules are private by default. Public integration surfaces should be deliberate.

Rules:

- `*.Domain`, `*.Application`, and `*.Infrastructure` are internal module layers.
- `*.Contracts` is the public boundary for DTOs and cross-module contracts.
- `Contracts` may depend only on stable shared abstractions such as `CodeCafe.SharedKernel` or `CodeCafe.BuildingBlocks`.
- One module must not reference another module's `Infrastructure`.
- Cross-module collaboration should happen through `Contracts`, explicit application interfaces, or host-level composition where appropriate.
- Adapters may call application entry points for the module they are serving, but they must not reach into persistence details or cross module internals.

Examples:

- `CodeCafe.Web` may call `Notes.Application` entry points for HTTP note workflows.
- `AI` may consume `Notes.Contracts` to understand note-facing integration shapes.
- `Notes.Infrastructure` must not reference `Code.Infrastructure`.

Anti-patterns:

- `CodeCafe.Web` querying a module `DbContext` directly for business behavior
- `Notes.Application` referencing `AI.Infrastructure`
- `Code.Contracts` referencing `Code.Application`

## Messaging Style

The default backend application style is handler-based.

Use:

- commands for writes
- queries for reads
- validators before handlers
- explicit orchestration in application layer

Actor-like thinking may be used for:

- long-running AI sessions
- background workflows
- stateful task execution

But the whole architecture is not actor-first.

## AI Architecture Rules

- AI is part of the product architecture from day one.
- AI is not modeled as a single chat box.
- AI should act through tools and use cases.
- AI must not write directly to the database.
- Important AI-triggered mutations should require explicit confirmation where appropriate.
- AI actions should be auditable.
- Microsoft Agent Framework belongs inside the AI module, not at the center of the whole application.

## Data and Persistence

- Each module should own its persistence concerns.
- Avoid a generic repository layer unless a specific abstraction is truly needed.
- Avoid forcing all modules into one giant shared DbContext model if it weakens boundaries.
- Keep persistence implementations in infrastructure.

## Testing Strategy

Test layers should include:

- architecture tests for dependency direction
- unit tests for domain and application rules
- integration tests for module workflows
- adapter tests for HTTP, MCP, and realtime boundaries where valuable

## Architectural Review Checklist

- Is this code in the right module?
- Is this concern in the right layer?
- Is an adapter doing business logic?
- Is one module reaching into another module's internals?
- Is AI bypassing application workflows?
- Is a shared abstraction being added too early?
- Will this choice still make sense when the frontend and product become much larger?
