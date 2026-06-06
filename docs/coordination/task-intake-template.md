# Task Intake Template

Use this template when sending a new requirement to the workflow coordinator.

## Requirement Input

```text
Requirement ID:
Title:

Product goal:

User-visible outcome:

Known constraints:

Suggested lane, if any:

Relevant modules or paths:

Cross-module contracts needed:

Reference sources to consult:

Acceptance criteria:

Deadline or priority:

Open questions:
```

## Coordinator Triage

For each incoming requirement, the coordinator should decide:

- Which lane owns the work.
- Whether an existing lane can take it or a new lane is needed.
- Whether the work must be split into vertical slices.
- Whether contracts, host wiring, adapter changes, package files, or solution files create conflict risk.
- Whether the requirement is clear enough to dispatch.
- Whether research, design, implementation, review, and merge should be split into separate child sessions.

If the requirement is unclear, ask the planning session for clarification instead of expanding the product direction independently.

## Dispatch Output Format

When the coordinator creates a child-session prompt, include:

- Lane name.
- Branch name.
- Worktree path.
- Base branch.
- Allowed paths.
- Forbidden paths.
- Specific task.
- Acceptance criteria.
- Required test/build commands.
- Required completion report fields.

The prompt must tell the child session to paste its completion report back to the coordinator session.

## Child Session Completion Requirements

Each child session must finish with:

- Commit hash.
- Diff summary.
- Tests and build results.
- Any forbidden paths touched.
- Any changed public contracts.
- Any merge blockers.
- Residual risks.

Completion reports should be pasted back to the coordinator session. The coordinator updates status and decides whether a separate merge session should be created.

## Coordinator Update Requirements

After a child session reports completion, update:

- `docs/coordination/session-registry.md`
- `docs/coordination/integration-board.md`
- `docs/coordination/module-contracts.md` if public contracts changed
- `docs/reference/reference-index.md` if new reference sources were added

Only recommend merge when relevant checks pass and conflict risks are understood.

## Merge Session Rule

Merges are coordinated work. A merge should be executed by a separate child session with its own worktree and branch after the coordinator decides the source branch is ready. The merge session must report conflicts, resolutions, final build/test results, and commit hash.
