# Integration Board

This board tracks merge readiness for active CodeCafe development lanes.

## Merge Policy

- `main` must remain buildable.
- Merge only branches with relevant passing build/test results.
- Merge one lane at a time.
- Re-run backend and/or frontend checks after each merge.
- Do not merge branches that changed conflict-risk surfaces without coordinator review.
- Child sessions must commit before their result is evaluated.
- Merge execution happens in a separate merge session/worktree only after coordinator approval.

## Current Lane Status

| Lane | Branch | Status | Merge Readiness | Blockers | Conflict Risk |
| --- | --- | --- | --- | --- | --- |
| `ai-agent-core` | `codex/ai-agent-core` | Planned | Not ready | Session not started | AI contracts, MCP adapter, Host wiring if requested |
| `web-shell` | `codex/web-shell` | Implementation complete for REQ-003 Web | Awaiting review | Review session required before merge; source commit `8d17aa61a2e525ee5e8e541c337c50be1bf9e36b`; final API binding depends on REQ-002/API follow-up | shared frontend structure, placeholder workspace client, future workspace API binding |
| `web-shell-review` | `codex/web-shell-review` | Ready to dispatch | Not ready | Review-only session not started | None if review remains read-only |
| `client-sdk-foundation` | `codex/client-sdk-foundation` | Conditional follow-up | Not ready | Keep deferred until REQ-002 and workspace API exposure are merged | shared client boundary shape, package files if generated tooling is proposed |
| `platform-workspace` | `codex/platform-workspace` | Needs fix after REQ-002 review | Not ready | Fix registration atomicity and concurrent first-workspace fallback before re-review | Platform contracts, EF migrations, registration flow, current workspace fallback |
| `platform-workspace-review` | `codex/platform-workspace-review` | Completed | Not ready | Review found two P2 issues requiring source-branch fixes | None; review remained read-only |
| `platform-workspace-entry-api` | `codex/platform-workspace-entry-api` | Deferred; likely needed after REQ-002 merge | Not ready | Wait for REQ-002 review and merge | Web adapter, Host wiring, Platform application query exposure |
| `notes-knowledge` | `codex/notes-knowledge` | Planned | Not ready | Session not started | Notes contracts, Web adapter endpoints, Host wiring |
| `code-workspace` | `codex/code-workspace` | Planned | Not ready | Session not started | Code contracts, AI integration, MCP tools |
| `avalonia-desktop` | `codex/avalonia-desktop` | Planned for REQ-003 | Not ready | Session not started; final API binding depends on REQ-002 | solution files, shared build props, desktop project placement, future workspace API contracts |

## Conflict-Risk Queue

These files and directories are likely to cause merge conflicts or cross-lane coupling:

- `CodeCafe.slnx`
- `Directory.Build.props`
- `src/Host/CodeCafe.Host/**`
- `src/BuildingBlocks/**`
- `src/Adapters/Web/**`
- `src/Adapters/Mcp/**`
- `src/Adapters/Realtime/**`
- `src/Modules/*/*.Contracts/**`
- `src/Modules/Platform/CodeCafe.Modules.Platform.Infrastructure/Persistence/Migrations/**`
- `src/Frontend/codecafe-web/package.json`
- `src/Frontend/codecafe-web/package-lock.json`
- `src/Desktop/**` when project files require solution or shared build changes
- `docs/coordination/**`

## Integration Gates

### Backend

Run from repository root when backend code changes:

```powershell
dotnet restore CodeCafe.slnx
dotnet build CodeCafe.slnx
dotnet test tests/Backend/UnitTests/CodeCafe.UnitTests.csproj
dotnet test tests/Backend/IntegrationTests/CodeCafe.IntegrationTests.csproj
dotnet test tests/Backend/ArchitectureTests/CodeCafe.ArchitectureTests.csproj
```

### Frontend

Run from `src/Frontend/codecafe-web` when frontend code changes:

```powershell
npm run type-check
npm run lint
npm run build
```

### Desktop

The desktop lane must define its own build command after the Avalonia project shape is approved. Until then, desktop branches are not merge-ready.

## Merge Order Notes

- Module-internal branches can be reviewed independently if they avoid host, adapter, and solution changes.
- Contract changes should be merged before adapter or frontend work depends on them.
- Host or adapter wiring should be a small follow-up branch after module contracts are stable.
- Platform workspace review found two true P2 issues: registration atomicity and concurrent first-workspace fallback. Fix `codex/platform-workspace` before re-review.
- The review's P1 coordination-doc finding was a diff-baseline false positive: merge-base diff shows `codex/platform-workspace` did not touch `docs/coordination/**`.
- Platform workspace should merge before Notes, Code, AI, Web, Desktop, or Mobile lanes require persisted workspace context.
- `platform-workspace-entry-api` should start after REQ-002 merge because no current workspace Web/API endpoint exists on `main` or in the REQ-002 branch.
- Web shell implementation is complete on `codex/web-shell`; review it before opening a merge session.
- Keep `client-sdk-foundation` deferred until Web/Desktop reviews show whether shared client-boundary unification is actually needed.
- REQ-003 Web/Desktop shell branches may be built in parallel with REQ-002 only if they use replaceable typed boundaries and do not assume missing endpoint paths.
- REQ-003 is not fully product-complete until current workspace contracts/API from REQ-002 are available and bound.
- Package and lockfile changes should be batched and reviewed intentionally.
