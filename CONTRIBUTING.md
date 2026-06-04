# Contributing to CodeCafe Next

CodeCafe Next is an AI Native workspace platform built for long-term growth.

This file is the contributor entry point. Detailed architecture and coding rules live under `docs/`.

## How We Work

- Low-cost models may help with scaffolding and repetitive implementation.
- Human contributors actively participate in design and coding decisions.
- Review is used to protect architecture, boundaries, and long-term maintainability.

## Read These First

- `docs/architecture/backend.md`
- `docs/architecture/frontend.md`
- `docs/best-practices/backend.md`
- `docs/best-practices/frontend.md`

## Contribution Rules

- Keep tasks small and reviewable.
- Follow the documented module and layer boundaries.
- Do not treat AI as a shortcut around application rules.
- Prefer explicit code over clever abstractions.
- If a rule is unclear, update the docs before scaling the pattern.

## Best Uses for Low-Cost Models

- solution and project scaffolding
- repetitive handlers and DTOs
- CRUD wiring
- test skeletons
- documentation drafts

Always review low-cost model output carefully when it affects:

- architecture
- module boundaries
- AI workflows
- cross-module dependencies
- shared abstractions
- security, authorization, and audit behavior

## Review Checkpoints

Request review:

- after creating or changing solution structure
- after the first implementation of a module
- before introducing cross-module calls
- when adding AI orchestration
- when adding realtime flows
- before merging a meaningful milestone
