# Session Registry

This registry tracks planned and active development lanes for the CodeCafe 24h parallel build workflow.

Coordinator rule: this session does not create child sessions. It only updates coordination documents and gives the user copy-ready prompts for new sessions.

## Global Worktree Rules

- Main branch: `main`
- Main repository path: `D:\Development\CodeCafe-Next`
- Suggested worktree root: `D:\Development\CodeCafe-Next.worktrees`
- Branch prefix: `codex/`
- One lane uses exactly one branch and one independent worktree.
- Child sessions must start by reporting their worktree path, branch, allowed paths, and forbidden paths.
- Child sessions must commit before reporting completion.
- Child sessions must paste a structured completion report back to the coordinator session so registry, contracts, and integration readiness can be updated.
- Only branches with passing relevant build/test results should be suggested for merge.
- Merge execution should use its own short-lived worktree and branch after the coordinator decides a branch is ready to merge.

## Lane Registry

| Lane | Branch | Suggested Worktree | Responsible Scope | Forbidden Scope | Status |
| --- | --- | --- | --- | --- | --- |
| `ai-agent-core` | `codex/ai-agent-core` | `D:\Development\CodeCafe-Next.worktrees\ai-agent-core` | `src/Modules/AI/**`, AI-facing tests, AI contract proposals | `src/Host/**`, `CodeCafe.slnx`, `Directory.Build.props`, `src/BuildingBlocks/**`, non-AI modules, frontend, package files without coordinator approval | Planned |
| `web-shell` | `codex/web-shell` | `D:\Development\CodeCafe-Next.worktrees\web-shell` | `src/Frontend/codecafe-web/src/**`, frontend tests, frontend architecture notes | backend modules, adapters, `src/Host/**`, `CodeCafe.slnx`, backend shared files, package or lockfile changes without coordinator approval | Planned |
| `platform-workspace` | `codex/platform-workspace` | `D:\Development\CodeCafe-Next.worktrees\platform-workspace` | `src/Modules/Platform/**`, Platform-focused backend tests, Platform workspace contracts | `src/Host/**`, `src/Adapters/**`, `CodeCafe.slnx`, `Directory.Build.props`, non-Platform modules, frontend, package files without coordinator approval | Planned for REQ-002 |
| `notes-knowledge` | `codex/notes-knowledge` | `D:\Development\CodeCafe-Next.worktrees\notes-knowledge` | `src/Modules/Notes/**`, Notes tests, Notes contract proposals | `src/Host/**`, `src/Adapters/**`, `CodeCafe.slnx`, `Directory.Build.props`, other modules, frontend without coordinator approval | Planned |
| `code-workspace` | `codex/code-workspace` | `D:\Development\CodeCafe-Next.worktrees\code-workspace` | `src/Modules/Code/**`, Code tests, Code contract proposals | `src/Host/**`, `src/Adapters/**`, `CodeCafe.slnx`, `Directory.Build.props`, other modules, frontend without coordinator approval | Planned |
| `avalonia-desktop` | `codex/avalonia-desktop` | `D:\Development\CodeCafe-Next.worktrees\avalonia-desktop` | future `src/Desktop/**`, future desktop tests, Avalonia client architecture notes | backend module internals, web frontend, host, solution files, package files, shared building blocks without coordinator approval | Planned |

## Conflict-Risk Surfaces

The following paths require explicit coordinator approval before a child session changes them:

- `CodeCafe.slnx`
- `Directory.Build.props`
- `src/Host/CodeCafe.Host/**`
- `src/BuildingBlocks/**`
- `src/Adapters/Web/**`
- `src/Adapters/Mcp/**`
- `src/Adapters/Realtime/**`
- `src/Modules/*/*.Contracts/**`
- `src/Frontend/codecafe-web/package.json`
- `src/Frontend/codecafe-web/package-lock.json`
- `docs/coordination/**`
- `docs/product/ai-work-os-blueprint.md`
- `docs/reference/reference-index.md`

## Child Session Completion Report

Each child session must report:

- Worktree path.
- Branch name.
- Responsible paths changed.
- Forbidden paths touched, or explicit statement that none were touched.
- Commit hash and commit message.
- Diff summary.
- Tests/build commands run and results.
- Public contracts added or changed.
- Database migrations added or changed.
- Follow-up integration lanes requested.
- Residual risks and integration notes.

## Copy-Ready Session Prompts

Use these prompts when creating child development sessions. Paste one prompt per new session.

### Prompt: ai-agent-core

```text
You are the ai-agent-core development session for CodeCafe-Next.

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\ai-agent-core
- Branch: codex/ai-agent-core
- Base branch: main

Immediately report:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths

Allowed primary paths:
- src/Modules/AI/**
- tests/Backend/UnitTests/** only for AI-focused tests
- tests/Backend/IntegrationTests/** only when testing AI workflows without changing Platform auth behavior

Forbidden without coordinator approval:
- CodeCafe.slnx
- Directory.Build.props
- src/Host/**
- src/BuildingBlocks/**
- src/Adapters/**
- src/Modules/Platform/**
- src/Modules/Notes/**
- src/Modules/Code/**
- src/Frontend/**
- package or lock files
- docs/coordination/**

Task:
Design and implement the first narrow AI module foundation for agent tasks and conversations, using existing module layering. Keep adapters and host wiring out of scope unless the coordinator explicitly approves a contract or integration follow-up.

Acceptance criteria:
- AI module has clear Domain/Application/Contracts shape for a minimal agent task or conversation concept.
- No direct database mutation by AI orchestration logic outside application workflows.
- Public contracts are minimal and documented in your completion report.
- Relevant unit tests are added where behavior exists.
- `dotnet build CodeCafe.slnx` passes from your worktree, or you explain exactly why it cannot run.
- You commit your work and report commit hash, diff summary, tests, and residual integration risks.
```

### Prompt: web-shell

```text
You are the web-shell development session for CodeCafe-Next.

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\web-shell
- Branch: codex/web-shell
- Base branch: main

Immediately report:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths

Allowed primary paths:
- src/Frontend/codecafe-web/src/**
- tests/Frontend/** if frontend tests are added
- docs/architecture/frontend.md only if documenting architecture decisions from implementation

Forbidden without coordinator approval:
- backend source under src/Modules/**
- src/Adapters/**
- src/Host/**
- CodeCafe.slnx
- Directory.Build.props
- src/Frontend/codecafe-web/package.json
- src/Frontend/codecafe-web/package-lock.json
- docs/coordination/**

Task:
Build the first browser workspace shell foundation using the existing Vite + React + TypeScript and Feature-Sliced Design structure. Keep the implementation focused on shell composition, routing readiness, and reusable UI/API/realtime foundations. Do not invent backend endpoints.

Acceptance criteria:
- App shell has clear slots for navigation, main workspace surface, and AI/context panel.
- FSD import direction remains respected.
- No raw backend API assumptions are scattered through pages.
- `npm run type-check` and `npm run build` pass from `src/Frontend/codecafe-web`, or failures are reported with exact causes.
- No package or lockfile changes unless coordinator approved them first.
- You commit your work and report commit hash, diff summary, tests, and residual integration risks.
```

### Prompt: platform-workspace

```text
You are the platform-workspace development session for CodeCafe-Next.

Requirement:
- Requirement ID: REQ-002
- Title: Establish AI Work OS Workspace Foundation

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\platform-workspace
- Branch: codex/platform-workspace
- Base branch: main

Immediately report to the user, before making code changes:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths
- whether your branch was created from main

Allowed primary paths:
- src/Modules/Platform/**
- tests/Backend/UnitTests/** only for Platform workspace-focused tests
- tests/Backend/IntegrationTests/** only for auth user -> workspace flow tests

Coordinator-approved risk paths for this task:
- src/Modules/Platform/CodeCafe.Modules.Platform.Contracts/** because REQ-002 requires WorkspaceId, current workspace context, and basic workspace response DTO.
- src/Modules/Platform/CodeCafe.Modules.Platform.Infrastructure/Persistence/Migrations/** if EF Core schema changes are required for Workspace persistence.

Forbidden without additional coordinator approval:
- CodeCafe.slnx
- Directory.Build.props
- src/Host/**
- src/Adapters/**
- src/BuildingBlocks/**
- src/Modules/Notes/**
- src/Modules/Code/**
- src/Modules/AI/**
- src/Frontend/**
- package or lock files
- docs/coordination/**
- docs/product/**
- docs/reference/**

Task:
Implement the first Platform-owned workspace foundation. A registered/authenticated user should be able to own or create a default personal workspace, and Platform should expose an application-layer query for the current workspace context. Keep complex team permissions, billing tenancy, organization management, controller endpoints, and Host wiring out of scope.

Implementation boundaries:
- Keep business logic in Platform Domain/Application/Infrastructure.
- Do not put business logic in Host or controllers.
- Preserve existing Platform auth behavior and tests.
- Prefer minimal contracts that future Notes, Code, AI, Web, Desktop, and Mobile lanes can consume.
- If an HTTP endpoint, adapter wiring, or Host registration is required, do not implement it in this branch; report the exact follow-up integration lane needed.

Required contract concepts:
- WorkspaceId
- Current workspace context
- Basic workspace response DTO

Acceptance criteria:
- Platform has Workspace domain/application/infrastructure foundation for a personal workspace.
- Registering a user can result in that user owning or being able to create a default workspace.
- There is an application use case/query for current workspace.
- Integration tests cover auth user -> workspace flow.
- Existing Platform auth flow remains passing.
- `dotnet build CodeCafe.slnx` passes from your worktree.
- `dotnet test tests/Backend/IntegrationTests/CodeCafe.IntegrationTests.csproj` passes from your worktree.
- Add or run focused unit tests if domain/application behavior is non-trivial.

Completion report required:
- Worktree path.
- Branch name.
- Commit hash and commit message.
- Responsible paths changed.
- Forbidden paths touched, or state "none".
- Public contracts added or changed, including file paths.
- Database migrations added or changed, including file paths.
- Diff summary.
- Exact build/test commands run and results.
- Whether REQ-002 acceptance criteria are fully met.
- Follow-up integration lanes requested, if any.
- Residual risks.
```

### Prompt: notes-knowledge

```text
You are the notes-knowledge development session for CodeCafe-Next.

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\notes-knowledge
- Branch: codex/notes-knowledge
- Base branch: main

Immediately report:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths

Allowed primary paths:
- src/Modules/Notes/**
- tests/Backend/UnitTests/** only for Notes-focused tests
- tests/Backend/IntegrationTests/** only for Notes module workflows that do not require adapter or host changes

Forbidden without coordinator approval:
- CodeCafe.slnx
- Directory.Build.props
- src/Host/**
- src/Adapters/**
- src/BuildingBlocks/**
- src/Modules/Platform/**
- src/Modules/Code/**
- src/Modules/AI/**
- src/Frontend/**
- docs/coordination/**

Task:
Design and implement the first narrow Notes knowledge foundation inside the Notes module. Focus on module-owned concepts and application use cases, not Web controllers or frontend UI.

Acceptance criteria:
- Notes module has a minimal domain/application shape for note or knowledge items.
- Contracts expose only stable DTOs needed for future adapters or AI tools.
- Module boundaries remain intact.
- Relevant unit tests are added where behavior exists.
- `dotnet build CodeCafe.slnx` passes from your worktree, or you explain exactly why it cannot run.
- You commit your work and report commit hash, diff summary, tests, and residual integration risks.
```

### Prompt: code-workspace

```text
You are the code-workspace development session for CodeCafe-Next.

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\code-workspace
- Branch: codex/code-workspace
- Base branch: main

Immediately report:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths

Allowed primary paths:
- src/Modules/Code/**
- tests/Backend/UnitTests/** only for Code-focused tests
- tests/Backend/IntegrationTests/** only for Code module workflows that do not require adapter or host changes

Forbidden without coordinator approval:
- CodeCafe.slnx
- Directory.Build.props
- src/Host/**
- src/Adapters/**
- src/BuildingBlocks/**
- src/Modules/Platform/**
- src/Modules/Notes/**
- src/Modules/AI/**
- src/Frontend/**
- docs/coordination/**

Task:
Design and implement the first narrow Code workspace foundation inside the Code module. Focus on repository/workspace concepts, code context contracts, and application behavior that can later be exposed through Web, MCP, or AI tools.

Acceptance criteria:
- Code module has a minimal domain/application shape for a code workspace or repository context.
- Contracts are small and suitable for future AI and adapter integration.
- No direct coupling to AI, Notes, Web, MCP, or Realtime internals.
- Relevant unit tests are added where behavior exists.
- `dotnet build CodeCafe.slnx` passes from your worktree, or you explain exactly why it cannot run.
- You commit your work and report commit hash, diff summary, tests, and residual integration risks.
```

### Prompt: avalonia-desktop

```text
You are the avalonia-desktop development session for CodeCafe-Next.

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\avalonia-desktop
- Branch: codex/avalonia-desktop
- Base branch: main

Immediately report:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths

Allowed primary paths:
- src/Desktop/** if a desktop scaffold is approved within this task
- tests/Desktop/** if desktop tests are added
- docs/architecture/** only for desktop architecture notes

Forbidden without coordinator approval:
- CodeCafe.slnx
- Directory.Build.props
- backend modules under src/Modules/**
- src/Adapters/**
- src/Host/**
- src/Frontend/**
- src/BuildingBlocks/**
- package or lock files
- docs/coordination/**

Task:
Prepare the Avalonia desktop lane foundation. Inspect the existing repository and local `Reference/Avalonia` checkout, then propose the minimal desktop project shape. Only create a scaffold if it can be kept inside `src/Desktop/**` without changing shared solution or host files; otherwise stop after documenting the exact integration proposal.

Acceptance criteria:
- Desktop lane reports the proposed Avalonia project structure and integration points.
- No solution, host, backend, frontend, or shared file changes are made without coordinator approval.
- If scaffolded, it is contained in `src/Desktop/**` and builds independently if possible.
- You run the relevant build command for any scaffold you create, or explain why no build was run.
- You commit your work and report commit hash, diff summary, tests, and residual integration risks.
```
