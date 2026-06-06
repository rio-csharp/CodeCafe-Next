# Module Contracts

This document tracks public module boundaries and cross-lane contract rules.

## Contract Rules

- Module internals are private by default.
- Public integration surfaces live in `src/Modules/<Module>/CodeCafe.Modules.<Module>.Contracts/**`.
- Contracts should contain DTOs, integration events, and stable request/response shapes.
- Contracts must not depend on sibling module `Domain`, `Application`, or `Infrastructure` projects.
- Adapters should call application behavior and map transport-specific requests/responses.
- AI must use tools and application workflows. It must not directly mutate another module's storage.
- Contract changes are conflict-risk changes and require coordinator tracking.

## Current Public Surfaces

| Module | Public Surface | Current State | Notes |
| --- | --- | --- | --- |
| Platform | `src/Modules/Platform/CodeCafe.Modules.Platform.Contracts/Auth/**` | Active | Registration, login, and current-user response contracts exist. |
| Notes | `src/Modules/Notes/CodeCafe.Modules.Notes.Contracts/**` | Empty or minimal | Future note/knowledge DTOs should start here. |
| Code | `src/Modules/Code/CodeCafe.Modules.Code.Contracts/**` | Empty or minimal | Future repository/workspace context contracts should start here. |
| AI | `src/Modules/AI/CodeCafe.Modules.AI.Contracts/**` | Empty or minimal | Future conversation, task, and tool-call contracts should start here. |

## Adapter Contract Expectations

### Web

- Web maps HTTP requests to application use cases.
- Web should not own module business logic.
- Web controller changes are integration work and should happen after module contracts are stable.

### MCP

- MCP tools should expose product capabilities through the same application workflows as Web.
- MCP must not bypass module application layers.
- AI-facing tool contracts should be reviewed with `ai-agent-core`.

### Realtime

- Realtime is expected for AI streaming, task progress, collaboration, and presence.
- Realtime contracts should be small and event-shaped.
- Long-running task progress should not be modeled as ordinary CRUD polling if streaming is required.

## Cross-Module Interaction Rules

- Platform auth and current user may be consumed by application workflows through explicit abstractions.
- Platform workspace identity and current workspace context are foundational contracts for all future product modules.
- Notes and Code should expose read/query contracts before AI depends on them.
- Notes, Code, AI, Projects, IM, Ledger, Web, Desktop, and Mobile should consume workspace context through Platform contracts or application abstractions, not Platform infrastructure.
- AI may propose actions against Notes or Code, but execution must go through those modules' application use cases.
- Host composition can wire modules together, but host must not contain orchestration logic.

## Pending Contract Decisions

| Decision | Needed By | Owner Lane | Status |
| --- | --- | --- | --- |
| Minimal AI conversation/task contract | MCP, Web shell, Realtime | `ai-agent-core` | Planned |
| WorkspaceId, current workspace context, basic workspace response DTO | Notes, Code, AI, Web shell, Desktop, Mobile | `platform-workspace` | Planned for REQ-002 |
| Minimal Notes knowledge item contract | Web shell, AI tools | `notes-knowledge` | Planned |
| Minimal Code workspace context contract | AI tools, MCP | `code-workspace` | Planned |
| Desktop client API consumption model | Avalonia shell | `avalonia-desktop` | Planned |

## REQ-002 Platform Workspace Contract Intent

The `platform-workspace` lane should introduce the smallest stable contract set needed to let the rest of the product attach user data to a personal workspace.

Expected concepts:

- `WorkspaceId`: stable workspace identity value exposed through Platform contracts.
- Current workspace context: identifies the authenticated user's active personal workspace for application workflows.
- Basic workspace response DTO: enough for Web/Desktop/Mobile shells to render the current workspace without depending on Platform internals.

Out of scope for REQ-002:

- Complex team roles and permissions.
- Multi-tenant billing constructs.
- Organization management.
- Non-Platform module persistence changes.
- Host or controller business logic.

## Contract Change Protocol

When a child session changes a contract:

1. Include the changed contract files in the completion report.
2. Explain which lanes may depend on the change.
3. Add or update tests for behavior that uses the contract.
4. Ask the coordinator to update this document.
5. Do not make unrelated adapter or host changes in the same branch unless approved.
