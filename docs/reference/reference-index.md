# Reference Index

This document tracks local reference source used by CodeCafe planning and implementation.

`Reference/` is ignored by Git. Nothing under `Reference/` should be committed.

## Current References

| Reference | Local Path | Intended Use | Primary Lanes |
| --- | --- | --- | --- |
| Microsoft Agent Framework | `Reference/agent-framework` | Agent architecture, orchestration patterns, tool planning ideas | `ai-agent-core` |
| Avalonia | `Reference/Avalonia` | Desktop UI architecture and Avalonia project conventions | `avalonia-desktop` |
| earendil-works/pi | `Reference/pi` | Local reference for AI/workspace product ideas and implementation patterns | coordinator, `web-shell`, `ai-agent-core` |
| earendil-works/pi mono variant | `Reference/pi-mono` | Alternate local reference for architecture comparison | coordinator |

## Usage Rules

- Reference code may inform architecture and implementation choices.
- Do not copy large code blocks or vendor source into CodeCafe without explicit review.
- Respect licenses and attribution requirements before reusing implementation details.
- Treat reference checkouts as local research material, not part of the product repository.
- If a child session uses a reference source, it must mention that in its completion report.

## Reference Intake

When adding a new reference source, record:

- Name.
- Local path.
- Source URL or origin if available.
- Why it was added.
- Which lanes may use it.
- Any license or reuse concerns.

## Open Reference Questions

- Which Microsoft Agent Framework patterns should become durable CodeCafe AI module contracts?
- Which Avalonia project shape best fits the existing .NET solution without disrupting backend work?
- Which `pi` patterns are product inspiration only, and which are implementation candidates?
