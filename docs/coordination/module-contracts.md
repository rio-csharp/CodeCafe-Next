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
| Platform | `src/Modules/Platform/CodeCafe.Modules.Platform.Contracts/Workspaces/**` | Implemented on `codex/platform-workspace`; pending review/merge | Adds `WorkspaceResponse` and `CurrentWorkspaceContextResponse`. |
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
| WorkspaceId, current workspace context, basic workspace response DTO | Notes, Code, AI, Web shell, Desktop, Mobile | `platform-workspace` | Implemented for REQ-002 on commit `a7575952c355df5d7fa2b0337d78b22ea92a714a`; pending review/merge |
| Cross-platform workspace client boundary | Web shell, Desktop, Mobile | `web-shell`, `avalonia-desktop`, possible `client-sdk-foundation` | Web boundary implemented on `codex/web-shell` with placeholder data; final shape depends on REQ-002 and API follow-up |
| Minimal Notes knowledge item contract | Web shell, AI tools | `notes-knowledge` | Planned |
| Minimal Code workspace context contract | AI tools, MCP | `code-workspace` | Planned |
| Desktop client API consumption model | Avalonia shell | `avalonia-desktop` | Planned |

## REQ-002 Platform Workspace Contract Intent

The `platform-workspace` lane should introduce the smallest stable contract set needed to let the rest of the product attach user data to a personal workspace.

Implemented concepts on `codex/platform-workspace`:

- `WorkspaceId`: domain value object for stable workspace identity.
- `WorkspaceResponse`: basic workspace response DTO.
- `CurrentWorkspaceContextResponse`: current workspace context response DTO.
- `GetCurrentWorkspaceContextQuery`: application query for the authenticated user's current workspace.
- `CurrentWorkspaceContextView` and `WorkspaceView`: application read models.

Out of scope for REQ-002:

- Complex team roles and permissions.
- Multi-tenant billing constructs.
- Organization management.
- Non-Platform module persistence changes.
- Host or controller business logic.

REQ-002 completion status:

- Source branch: `codex/platform-workspace`
- Source commit: `a7575952c355df5d7fa2b0337d78b22ea92a714a`
- Verification reported by child session: `dotnet build CodeCafe.slnx` passed; `dotnet test tests/Backend/IntegrationTests/CodeCafe.IntegrationTests.csproj` passed with 6 tests.
- Forbidden paths touched: none reported and coordinator spot-check found none.
- Residual risk: registration user/workspace creation is not wrapped in an explicit transaction; query fallback mitigates missing workspace rows, but concurrent first-workspace creation may race on the unique index.
- Current workspace Web/API endpoint: absent; dispatch `platform-workspace-entry-api` after review and merge unless review changes that judgment.

## REQ-003 Cross-Platform Workspace Entry Contract Intent

The `web-shell` and `avalonia-desktop` lanes should consume workspace concepts through typed boundaries while REQ-002 is still in flight.

Expected client-side concepts:

- `WorkspaceId`: opaque identifier matching Platform intent.
- `CurrentWorkspaceResponse`: display-ready current workspace data.
- Authenticated current user/session state: enough for a logged-in shell to request or display current workspace context.

Rules:

- If Platform workspace contracts/API are absent from the base branch, clients may create temporary local types and adapters.
- Temporary client types must be isolated behind API/client/service boundaries so they can be replaced by real Platform contracts later.
- Raw fetch calls must not be scattered across Web pages or widgets.
- Desktop API access must be behind a service boundary, not embedded in views.
- A separate `client-sdk-foundation` lane should start only after REQ-002 and workspace API exposure are merged, and after Web/Desktop reviews show duplicated client-boundary work that should be unified.

REQ-003 Web completion status:

- Source branch: `codex/web-shell`
- Source commit: `8d17aa61a2e525ee5e8e541c337c50be1bf9e36b`
- Verification reported by child session: `npm run type-check` passed; `npm run build` passed; Vite render sanity check passed with no browser console errors.
- Forbidden paths touched: none reported and coordinator spot-check found none.
- Workspace client boundary: `src/Frontend/codecafe-web/src/processes/workspace/**`
- Placeholder status: local placeholder current workspace data remains and must be replaced after REQ-002/API follow-up.

## Contract Change Protocol

When a child session changes a contract:

1. Include the changed contract files in the completion report.
2. Explain which lanes may depend on the change.
3. Add or update tests for behavior that uses the contract.
4. Ask the coordinator to update this document.
5. Do not make unrelated adapter or host changes in the same branch unless approved.
