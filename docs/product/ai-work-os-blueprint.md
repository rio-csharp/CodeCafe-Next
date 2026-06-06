# CodeCafe 24h AI Work OS Blueprint

## Product Goal

CodeCafe is an AI-native work operating system for builders. It should combine notes, code understanding, workspace context, and agentic execution into one coherent product surface.

The target is not a notes app with a chatbot attached. CodeCafe should become a workspace where users can capture knowledge, work with code, ask AI to reason over local context, and turn AI suggestions into auditable actions.

Workspace is the product root. Notes, Code, AI, Projects, IM, Ledger, and future modules should attach to a user's current workspace instead of behaving as isolated pages or unrelated feature islands.

## Current Project Facts

- Repository: `CodeCafe-Next`
- Main branch: `main`
- Backend: .NET 10 modular monolith
- Frontend: Vite, React, TypeScript
- Desktop/mobile plan: Avalonia
- Current modules: `Platform`, `Notes`, `Code`, `AI`
- Current adapters: `Web`, `Mcp`, `Realtime`
- Platform Auth already includes registration, login, current user, cookie auth, EF Core SQLite, and integration tests.
- `Reference/` is ignored by Git and contains local reference source checkouts.

## End-Side Split

### Backend

The backend is the source of truth for domain behavior and long-running workflows.

Responsibilities:

- Own module boundaries and persistence.
- Keep use cases in application layers.
- Expose stable contracts through module `Contracts` projects.
- Serve equivalent product workflows through Web, MCP, and Realtime adapters.
- Keep `CodeCafe.Host` as the composition root, not a business logic layer.

### Web

The web app is the primary browser workspace.

Responsibilities:

- Provide a workspace shell with navigation, content surfaces, and AI surfaces.
- Use Feature-Sliced Design boundaries.
- Centralize API and realtime access in shared/process layers.
- Avoid embedding backend assumptions directly in page components.

### Desktop and Mobile

Avalonia is the planned desktop/mobile application path.

Responsibilities:

- Reuse backend contracts and product concepts where possible.
- Prepare for local workspace context, local files, and offline-aware behavior.
- Avoid forcing desktop-specific concerns into backend modules.

### AI and Tool Surfaces

AI should be available through product workflows, MCP tools, realtime task progress, and UI surfaces.

Responsibilities:

- Treat AI as a first-class module.
- Make actions explicit, observable, and auditable.
- Use application workflows and tool boundaries instead of direct storage mutation.
- Support streaming, task progress, and user confirmation where risk is meaningful.

## Core Modules

### Platform

Owns identity, auth, workspace context, session context, audit, and future workspace-level platform concerns. Platform is not a generic helper bucket.

### Notes

Owns notes, pages, knowledge structures, and future knowledge graph behavior.

### Code

Owns repository reading, code workspace state, code indexing, and code understanding surfaces.

### AI

Owns conversations, agent tasks, orchestration, tool execution, planning, and integration with Microsoft Agent Framework.

## AI Native Principles

- AI is a product primitive, not an endpoint wrapper.
- AI reads and acts through bounded tools and application use cases.
- AI-triggered mutations should be auditable.
- Risky or destructive actions require explicit confirmation.
- Agent progress should be streamable through realtime surfaces.
- Prompts, plans, tool calls, and outcomes should be traceable enough for debugging.
- Module contracts must describe what AI may read, propose, and mutate.
- Reference source may inform architecture, but ignored `Reference/` files must not be copied into the product without review.

## Workspace Foundation Principles

- A signed-in user should enter a current workspace, not a disconnected feature page.
- The first workspace slice supports personal workspaces owned by the authenticated user.
- Complex team permissions, billing tenancy, and organization management are out of scope for the first workspace foundation.
- Workspace identity and current workspace context belong in Platform.
- Other modules should depend on stable Platform contracts for workspace identity and context, not on Platform infrastructure.
- Host and controllers may expose workspace behavior later, but business rules must remain inside Platform application workflows.

## 24h Parallel Build Model

The project now uses a coordinator plus multiple development lanes:

- The coordinator receives requirements, assigns lanes, updates coordination docs, and produces child-session prompts.
- Each development lane uses one branch and one independent git worktree.
- `main` must remain buildable.
- Public contracts, host wiring, solution files, shared building blocks, and package or lock files are conflict-risk surfaces.
- Large requirements are split into vertical slices before implementation.
- A child session is complete only after it commits, reports diff summary, test results, and residual risk.

## First Development Lanes

- `ai-agent-core`: AI module, agent task/conversation core, MCP-facing tool boundaries.
- `web-shell`: browser workspace shell, routing, shared UI/API/realtime foundations.
- `platform-workspace`: Platform-owned workspace identity and current workspace foundation.
- `notes-knowledge`: Notes module knowledge model and note workflows.
- `code-workspace`: Code module repository/workspace model and code-understanding workflows.
- `avalonia-desktop`: Avalonia client foundation and desktop integration plan.

## Reference Sources

Current local reference checkouts:

- `Reference/agent-framework`: Microsoft Agent Framework reference.
- `Reference/Avalonia`: Avalonia reference.
- `Reference/pi`: earendil-works/pi reference.
- `Reference/pi-mono`: local mono reference variant.

Reference source is local-only and ignored by Git. Coordination details live in `docs/reference/reference-index.md`.
