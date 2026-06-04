# Backend Best Practices

## Philosophy

We optimize for:

- clear module boundaries
- explicit use-case flow
- long-term maintainability
- safe AI integration
- simple code that is easy to review

We do not optimize for:

- clever abstractions
- generic enterprise ceremony
- hiding behavior in framework magic
- premature cross-module reuse

## Architectural Rules

- Follow the modular monolith structure defined in `docs/architecture/backend.md`.
- Put business logic in module application flows, not in adapters.
- Keep HTTP, MCP, and realtime layers thin.
- Keep AI workflows inside the AI module and route actions through tools and use cases.
- Do not let one module depend on another module's infrastructure.

## SOLID in Practice

Use SOLID as a review tool, not as an excuse to create extra layers.

### Single Responsibility

- A handler should do one use case.
- A class should have one reason to change.
- A file that coordinates validation, persistence, mapping, and transport usually needs splitting.

### Open/Closed

- Prefer extension through new handlers, policies, and implementations.
- Do not keep modifying giant shared services for unrelated features.

### Liskov

- Avoid inheritance-heavy service hierarchies.
- Prefer composition over abstract base classes unless the abstraction is stable and truly needed.

### Interface Segregation

- Keep interfaces small and use-case-driven.
- Avoid wide service interfaces such as `INotesService` with many unrelated responsibilities.

### Dependency Inversion

- Application defines the interfaces it needs.
- Infrastructure implements them.
- Adapters depend on application entry points, not concrete persistence details.

## File and Class Size

Rules of thumb:

- Be cautious when a file passes 150 lines.
- Strongly question files above 250 lines.
- A handler should usually stay small enough to understand in one screenful.
- Large DTO collections in one file should be split by use case or slice.

These are review heuristics, not absolute laws. The point is readability and change isolation.

## Handler Rules

- One command or query per file.
- One handler per use case.
- Keep orchestration explicit.
- Validation should happen before the core handler logic when possible.
- Handlers should not contain transport concerns.
- Handlers should not know about `HttpContext`, SignalR hub state, or MCP request objects.

## Adapter Rules

### HTTP

Endpoints should:

1. bind request
2. call the application layer
3. map the result

Endpoints should not:

- query the database directly for business behavior
- branch through business rules
- duplicate validation logic

### MCP

- MCP tools should call the same application behavior as HTTP when serving the same use case.
- Do not create MCP-only business rules unless the product explicitly requires different behavior.

### Realtime

- Hubs should coordinate delivery, not own business logic.
- Push task and stream updates from application workflows or dedicated realtime services.

## Persistence Rules

- Keep EF Core and storage concerns in infrastructure.
- Avoid generic repository and generic unit-of-work layers unless a concrete need exists.
- Prefer module-owned persistence models over giant shared persistence abstractions.
- Keep transaction boundaries explicit.

## Shared Code Rules

- `SharedKernel` should stay small and stable.
- `BuildingBlocks` should not become a hidden second application layer.
- If code only helps one module, keep it there.
- Do not create a `Common` or `Utils` folder for unrelated leftovers.

## Naming Rules

- Use names that describe the business action.
- Prefer `CreateNoteCommand` over vague names like `NoteProcessor`.
- Prefer `GetWorkspaceTreeQuery` over `WorkspaceHelper`.
- Avoid suffix inflation such as `Manager`, `Processor`, `Utility`, `Helper` unless the role is truly precise.

## Error Handling

- Use domain-specific exceptions or result types intentionally.
- Do not swallow exceptions and return vague failure states.
- Log with enough context to debug production issues.
- Avoid leaking transport-specific error shaping into domain or application layers.

## AI-Specific Rules

- AI must not directly write to the database.
- AI-triggered writes should pass through application use cases.
- Important mutations should support confirmation and audit.
- Prompt handling, tool registration, and agent orchestration belong in the AI module.
- Do not bury AI behavior inside random services or controllers.

## Testing Rules

Must test:

- domain rules
- application behaviors
- cross-module integration points
- critical AI action safety paths
- regressions for fixed bugs

Recommended test layers:

- architecture tests
- unit tests
- integration tests

Do not chase coverage metrics blindly. Test for confidence and change safety.

## Review Checklist

- Is business logic leaking into adapters?
- Is a module boundary being crossed improperly?
- Is a new abstraction solving a real problem or just adding ceremony?
- Is a file too large or carrying multiple responsibilities?
- Is a handler doing more than one use case?
- Is AI bypassing confirmation, audit, or application workflows?
- Are tests missing for important logic changes?
