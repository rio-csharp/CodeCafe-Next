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
| `web-shell` | `codex/web-shell` | Planned | Not ready | Session not started | package files, shared frontend structure, future API contracts |
| `platform-workspace` | `codex/platform-workspace` | Planned for REQ-002 | Not ready | Session not started | Platform contracts, EF migrations, future Web/Host exposure |
| `notes-knowledge` | `codex/notes-knowledge` | Planned | Not ready | Session not started | Notes contracts, Web adapter endpoints, Host wiring |
| `code-workspace` | `codex/code-workspace` | Planned | Not ready | Session not started | Code contracts, AI integration, MCP tools |
| `avalonia-desktop` | `codex/avalonia-desktop` | Planned | Not ready | Session not started | solution files, shared build props, desktop project placement |

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
- Platform workspace should merge before Notes, Code, AI, Web, Desktop, or Mobile lanes require persisted workspace context.
- Package and lockfile changes should be batched and reviewed intentionally.
