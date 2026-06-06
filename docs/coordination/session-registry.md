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
- Review-only child sessions must still use an independent worktree and branch, but they must not edit files or create commits unless the coordinator explicitly changes the task.

## Lane Registry

| Lane | Branch | Suggested Worktree | Responsible Scope | Forbidden Scope | Status |
| --- | --- | --- | --- | --- | --- |
| `ai-agent-core` | `codex/ai-agent-core` | `D:\Development\CodeCafe-Next.worktrees\ai-agent-core` | `src/Modules/AI/**`, AI-facing tests, AI contract proposals | `src/Host/**`, `CodeCafe.slnx`, `Directory.Build.props`, `src/BuildingBlocks/**`, non-AI modules, frontend, package files without coordinator approval | Planned |
| `web-shell` | `codex/web-shell` | `D:\Development\CodeCafe-Next.worktrees\web-shell` | `src/Frontend/codecafe-web/src/**`, frontend tests, frontend architecture notes | backend modules, adapters, `src/Host/**`, `CodeCafe.slnx`, backend shared files, package or lockfile changes without coordinator approval | Completed for REQ-003 Web; pending review |
| `web-shell-review` | `codex/web-shell-review` | `D:\Development\CodeCafe-Next.worktrees\web-shell-review` | Review `codex/web-shell` against `main`; no implementation changes | All source/docs changes are forbidden unless coordinator converts the session into a fix task | Ready to dispatch |
| `client-sdk-foundation` | `codex/client-sdk-foundation` | `D:\Development\CodeCafe-Next.worktrees\client-sdk-foundation` | Future shared client boundary after Platform workspace contract/API stabilizes | Backend modules, adapters, host, solution files, package files, desktop scaffold, frontend feature work without coordinator approval | Conditional follow-up, do not start yet |
| `platform-workspace` | `codex/platform-workspace` | `D:\Development\CodeCafe-Next.worktrees\platform-workspace` | `src/Modules/Platform/**`, Platform-focused backend tests, Platform workspace contracts | `src/Host/**`, `src/Adapters/**`, `CodeCafe.slnx`, `Directory.Build.props`, non-Platform modules, frontend, package files without coordinator approval | Needs fix after REQ-002 review |
| `platform-workspace-review` | `codex/platform-workspace-review` | `D:\Development\CodeCafe-Next.worktrees\platform-workspace-review` | Review `codex/platform-workspace` against `main`; no implementation changes | All source/docs changes are forbidden unless coordinator converts the session into a fix task | Completed; fix required |
| `platform-workspace-entry-api` | `codex/platform-workspace-entry-api` | `D:\Development\CodeCafe-Next.worktrees\platform-workspace-entry-api` | Conditional Web/Host exposure for current workspace after REQ-002 merge | Platform domain rewrite, non-Platform modules, frontend, desktop, package files, shared building blocks without coordinator approval | Deferred; likely needed after REQ-002 merge |
| `notes-knowledge` | `codex/notes-knowledge` | `D:\Development\CodeCafe-Next.worktrees\notes-knowledge` | `src/Modules/Notes/**`, Notes tests, Notes contract proposals | `src/Host/**`, `src/Adapters/**`, `CodeCafe.slnx`, `Directory.Build.props`, other modules, frontend without coordinator approval | Planned |
| `code-workspace` | `codex/code-workspace` | `D:\Development\CodeCafe-Next.worktrees\code-workspace` | `src/Modules/Code/**`, Code tests, Code contract proposals | `src/Host/**`, `src/Adapters/**`, `CodeCafe.slnx`, `Directory.Build.props`, other modules, frontend without coordinator approval | Planned |
| `avalonia-desktop` | `codex/avalonia-desktop` | `D:\Development\CodeCafe-Next.worktrees\avalonia-desktop` | future `src/Desktop/**`, future desktop tests, Avalonia client architecture notes | backend module internals, web frontend, host, solution files, package files, shared building blocks without coordinator approval | Completed for REQ-003 Desktop; pending review |
| `avalonia-desktop-review` | `codex/avalonia-desktop-review` | `D:\Development\CodeCafe-Next.worktrees\avalonia-desktop-review` | Review `codex/avalonia-desktop` against `main`; no implementation changes | All source/docs changes are forbidden unless coordinator converts the session into a fix task | Ready to dispatch |

## Deferred Child Sessions

Do not start these sessions until the coordinator explicitly reactivates them.

| Lane | Branch | Why Deferred | Reactivation Trigger |
| --- | --- | --- | --- |
| `client-sdk-foundation` | `codex/client-sdk-foundation` | REQ-002 workspace contract/API is not merged yet; `web-shell` currently uses an isolated placeholder client boundary. Starting now would still force shared SDK shape guesses. | Start after REQ-002 and the workspace API exposure lane are merged, and after Web/Desktop review reports show duplicated client-boundary work that should be unified. |
| `platform-workspace-entry-api` | `codex/platform-workspace-entry-api` | REQ-002 provides application-layer workspace foundation but no Web/Controller endpoint was found for current workspace. Starting before REQ-002 review/merge would stack integration work on an unmerged branch. | Start after REQ-002 is reviewed and merged, unless review finds the endpoint already exists or rejects the branch. |
| `desktop-solution-wiring` | `codex/desktop-solution-wiring` | `avalonia-desktop` intentionally avoided `CodeCafe.slnx`, shared build props, and CI wiring. Starting now would touch conflict-risk files before review/merge. | Start after `avalonia-desktop` review and merge if coordinator approves adding desktop project to solution/CI. |

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

Requirement:
- Requirement ID: REQ-003
- Title: Cross-Platform Workspace Entry Vertical Slice

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\web-shell
- Branch: codex/web-shell
- Base branch: origin/main

Immediately report before making code changes:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths
- whether your branch was created from the latest origin/main

Allowed primary paths:
- src/Frontend/codecafe-web/src/**
- tests/Frontend/** if frontend tests are added
- docs/architecture/frontend.md only if documenting architecture decisions from implementation

Read-only context:
- docs/coordination/module-contracts.md
- docs/product/ai-work-os-blueprint.md
- src/Modules/Platform/CodeCafe.Modules.Platform.Contracts/** if present in your base

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
Implement the Web side of REQ-003. Build a visible workspace shell so an authenticated user lands in an AI Work OS workspace experience rather than an isolated page. Keep this to layout, routing readiness, current-workspace display, and centralized typed client boundary. Do not implement Notes, AI, Code, or backend endpoints.

Dependency handling:
- REQ-003 depends on REQ-002 Platform workspace contracts/API.
- Do not modify the already dispatched REQ-002 work.
- If REQ-002 is not merged into your base, create a replaceable typed boundary for `WorkspaceId` and `CurrentWorkspaceResponse` under shared/process-level frontend code.
- Do not hardcode final backend endpoint paths that are not present.
- If you need placeholder data, isolate it behind the workspace client boundary and call it out in your completion report.
- Do not scatter raw `fetch` calls across pages or widgets.

Expected Web shell:
- left navigation
- main workspace surface
- AI/context panel placeholder
- current workspace display
- authenticated/session-aware entry shape using existing auth concepts where available

Acceptance criteria:
- App shell has clear slots for navigation, main workspace surface, and AI/context panel.
- Current workspace is displayed through a typed boundary, even if backed by a temporary placeholder before REQ-002 is merged.
- FSD import direction remains respected.
- No raw backend API assumptions are scattered through pages.
- `npm run type-check` passes from `src/Frontend/codecafe-web`.
- `npm run build` passes from `src/Frontend/codecafe-web`.
- No package or lockfile changes unless coordinator approved them first.
- You commit your work.

Completion report required:
- Worktree path.
- Branch name.
- Commit hash and commit message.
- Responsible paths changed.
- Forbidden paths touched, or state "none".
- Workspace client boundary files added or changed.
- Whether any placeholder workspace data remains.
- Diff summary.
- Exact type-check/build/test commands run and results.
- Whether Web REQ-003 acceptance criteria are fully met.
- Follow-up integration lanes requested, especially if a real workspace API binding is needed after REQ-002.
- Residual risks.
```

### Prompt: web-shell-review

```text
You are the web-shell-review session for CodeCafe-Next.

Review target:
- Requirement ID: REQ-003
- Source branch: codex/web-shell
- Source commit: 8d17aa61a2e525ee5e8e541c337c50be1bf9e36b
- Base branch: origin/main

Create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\web-shell-review
- Branch: codex/web-shell-review
- Base branch: origin/main

This is review-only:
- Do not edit files.
- Do not stage files.
- Do not commit.
- Do not merge.
- Do not rewrite the source branch.

Review scope:
- Compare `codex/web-shell` against `main`.
- Focus on bugs, regressions, missing tests, FSD boundary violations, raw fetch/API leakage, layout issues, accessibility basics, and merge blockers.
- Verify no forbidden paths were changed.
- Confirm placeholder workspace data is isolated behind a typed boundary and no final backend endpoint paths were hardcoded.
- Confirm the Web workspace shell includes left navigation, current workspace display, main workspace surface, and AI/context panel placeholder.

Suggested commands:
- `git fetch origin`
- `git diff --name-only main..codex/web-shell`
- `git diff main..codex/web-shell`
- `cd src/Frontend/codecafe-web`
- `npm run type-check`
- `npm run build`

Completion report back to coordinator must include:
- Worktree path.
- Review branch.
- Source branch and commit reviewed.
- Findings first, ordered by severity, with file/line references where possible.
- Whether this branch is recommended to merge, needs fixes, or needs more testing.
- Commands run and results.
- Whether forbidden paths were touched by the source branch.
- Whether placeholder workspace data is properly isolated.
- Whether `client-sdk-foundation` should remain deferred.
- Residual risks.
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

### Prompt: platform-workspace-review

```text
You are the platform-workspace-review session for CodeCafe-Next.

Review target:
- Requirement ID: REQ-002
- Source branch: codex/platform-workspace
- Source commit: a7575952c355df5d7fa2b0337d78b22ea92a714a
- Base branch: main / origin/main

Before reviewing, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\platform-workspace-review
- Branch: codex/platform-workspace-review
- Base branch: origin/main

Immediately report before reviewing:
- actual worktree path
- actual branch
- source branch and source commit being reviewed
- that this is review-only

Review-only rules:
- Do not edit files.
- Do not stage files.
- Do not commit.
- Do not merge.
- Do not rewrite the source branch.

Review scope:
- Compare `codex/platform-workspace` against `main`.
- Focus on bugs, behavioral regressions, missing tests, architectural boundary violations, and merge blockers.
- Pay special attention to Platform contracts, EF migration/model snapshot, registration now creating a workspace, current workspace fallback creation, repository save behavior, and the reported residual risk around concurrent first-workspace creation.
- Verify forbidden paths were not changed.
- Confirm whether Web/API exposure for current workspace is absent and should remain a follow-up lane.

Suggested commands:
- `git fetch origin`
- `git diff --name-only main..codex/platform-workspace`
- `git diff main..codex/platform-workspace`
- `dotnet build CodeCafe.slnx`
- `dotnet test tests/Backend/IntegrationTests/CodeCafe.IntegrationTests.csproj`

Completion report back to coordinator must include:
- Worktree path.
- Review branch.
- Source branch and commit reviewed.
- Findings first, ordered by severity, with file/line references where possible.
- Explicit statement whether this branch is recommended to merge, needs fixes, or needs more testing.
- Commands run and results.
- Whether forbidden paths were touched by the source branch.
- Whether `platform-workspace-entry-api` should be started after merge.
- Residual risks.
```

### Prompt: platform-workspace-fix

```text
You are continuing the platform-workspace development session for CodeCafe-Next.

Requirement:
- Requirement ID: REQ-002
- Branch: codex/platform-workspace
- Worktree: D:\Development\CodeCafe-Next.worktrees\platform-workspace
- Previous commit reviewed: a7575952c355df5d7fa2b0337d78b22ea92a714a

Before editing, report:
- actual worktree path
- actual branch
- current HEAD commit
- allowed paths
- forbidden paths

Allowed paths:
- src/Modules/Platform/**
- tests/Backend/UnitTests/** only for Platform workspace-focused tests
- tests/Backend/IntegrationTests/** only for auth user -> workspace flow tests

Forbidden without coordinator approval:
- docs/coordination/**
- docs/product/**
- docs/reference/**
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

Review result to act on:
- P1 from review about forbidden coordination docs is treated by coordinator as a diff-baseline false positive. Do not modify coordination docs.
- P2 true issue: registration is not atomic across user/workspace creation. Fix so user registration and default personal workspace creation cannot leave a user without a workspace if workspace persistence fails.
- P2 true issue: concurrent first-workspace fallback can throw on the unique `(OwnerUserId, Kind)` index. Fix so concurrent requests resolve the existing workspace or otherwise fail gracefully without breaking current workspace context.

Task:
Patch the existing `codex/platform-workspace` branch to address the two P2 issues. Keep scope narrow. Do not add Web/API endpoints. Do not touch Host, adapters, frontend, solution files, or coordination docs.

Acceptance criteria:
- Registration user + default workspace creation is atomic or otherwise cannot leave partial state on workspace insert failure.
- Current workspace fallback handles concurrent creation of the default personal workspace.
- Existing Platform auth flow tests still pass.
- Add or update focused integration/unit tests for the fixed behavior if practical.
- `dotnet build CodeCafe.slnx` passes.
- `dotnet test tests/Backend/IntegrationTests/CodeCafe.IntegrationTests.csproj` passes.
- Commit the fix on `codex/platform-workspace`.

Completion report back to coordinator must include:
- Worktree path.
- Branch name.
- New commit hash and commit message.
- Changed paths.
- Forbidden paths touched, or "none".
- How each P2 finding was fixed.
- Tests/build commands run and results.
- Whether REQ-002 is now ready for re-review.
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

Requirement:
- Requirement ID: REQ-003
- Title: Cross-Platform Workspace Entry Vertical Slice

Before editing code, create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\avalonia-desktop
- Branch: codex/avalonia-desktop
- Base branch: origin/main

Immediately report before making code changes:
- actual worktree path
- actual branch
- allowed paths
- forbidden paths
- whether your branch was created from the latest origin/main

Allowed primary paths:
- src/Desktop/**
- tests/Desktop/** if desktop tests are added
- docs/architecture/** only for desktop architecture notes

Read-only context:
- docs/coordination/module-contracts.md
- docs/product/ai-work-os-blueprint.md
- Reference/Avalonia/** if present locally

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
Implement the Desktop side of REQ-003. Create an Avalonia workspace shell foundation that presents CodeCafe as a desktop AI Work OS workbench centered on the current workspace. Keep this to desktop shell structure and typed workspace boundary. Do not implement Notes, AI, Code, backend endpoints, solution integration, or host wiring.

Dependency handling:
- REQ-003 depends on REQ-002 Platform workspace contracts/API.
- Do not modify the already dispatched REQ-002 work.
- If REQ-002 is not merged into your base, create a replaceable local desktop contract for `WorkspaceId` and `CurrentWorkspaceResponse`.
- Do not hardcode final backend endpoint paths that are not present.
- If you need placeholder workspace data, isolate it behind a desktop workspace client/service boundary and call it out in your completion report.

Expected Desktop shell:
- desktop window or root view centered on current workspace
- left navigation
- main workspace surface
- AI/context panel placeholder
- current workspace display

Implementation boundaries:
- Keep all scaffold/code inside `src/Desktop/**` unless coordinator approves otherwise.
- Do not modify `CodeCafe.slnx`.
- Do not modify `Directory.Build.props`.
- Do not modify backend, web frontend, host, adapters, or shared building blocks.
- If a usable Avalonia scaffold cannot be created without solution/shared changes, stop after documenting the exact required follow-up instead of forcing the change.

Acceptance criteria:
- Desktop has an Avalonia workspace shell foundation under `src/Desktop/**`, or a precise blocked report explaining why it cannot be scaffolded without forbidden files.
- Shell concepts are centered on Workspace, not an isolated page.
- Current workspace is displayed through a typed desktop boundary, even if backed by a temporary placeholder before REQ-002 is merged.
- No solution, host, backend, frontend, or shared file changes are made without coordinator approval.
- If scaffolded, `dotnet build` for the desktop project passes, or exact failure causes are reported.
- You commit your work.

Completion report required:
- Worktree path.
- Branch name.
- Commit hash and commit message.
- Responsible paths changed.
- Forbidden paths touched, or state "none".
- Desktop project/scaffold path.
- Workspace client/service boundary files added or changed.
- Whether any placeholder workspace data remains.
- Diff summary.
- Exact build/test commands run and results.
- Whether Desktop REQ-003 acceptance criteria are fully met.
- Follow-up integration lanes requested, especially if solution integration or real workspace API binding is needed after REQ-002.
- Residual risks.
```

### Prompt: avalonia-desktop-review

```text
You are the avalonia-desktop-review session for CodeCafe-Next.

Review target:
- Requirement ID: REQ-003
- Source branch: codex/avalonia-desktop
- Source commit: 0024ab90924ff45d9d06a9f7b2f22f6be30e1582
- Base branch: origin/main

Create or switch to an independent git worktree and branch:
- Base repository: D:\Development\CodeCafe-Next
- Worktree path: D:\Development\CodeCafe-Next.worktrees\avalonia-desktop-review
- Branch: codex/avalonia-desktop-review
- Base branch: origin/main

This is review-only:
- Do not edit files.
- Do not stage files.
- Do not commit.
- Do not merge.
- Do not rewrite the source branch.

Review scope:
- Compare `codex/avalonia-desktop` against `main`.
- Focus on bugs, regressions, missing tests, Avalonia project validity, layout issues, service-boundary quality, placeholder isolation, and merge blockers.
- Verify no forbidden paths were changed.
- Confirm all implementation is under `src/Desktop/CodeCafe.Desktop/**`.
- Confirm the desktop shell includes left navigation, current workspace display, main workspace surface, and AI/context panel placeholder.
- Confirm placeholder workspace data is isolated behind a desktop service/client boundary and no final backend endpoint paths were hardcoded.
- Confirm whether `desktop-solution-wiring` should remain deferred until after merge.

Suggested commands:
- `git fetch origin`
- `git diff --name-only main..codex/avalonia-desktop`
- `git diff main..codex/avalonia-desktop`
- `dotnet build src/Desktop/CodeCafe.Desktop/CodeCafe.Desktop.csproj`

Completion report back to coordinator must include:
- Worktree path.
- Review branch.
- Source branch and commit reviewed.
- Findings first, ordered by severity, with file/line references where possible.
- Whether this branch is recommended to merge, needs fixes, or needs more testing.
- Commands run and results.
- Whether forbidden paths were touched by the source branch.
- Whether placeholder workspace data is properly isolated.
- Whether `desktop-solution-wiring` should remain deferred.
- Residual risks.
```
