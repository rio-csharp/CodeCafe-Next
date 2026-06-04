# Frontend Architecture

## Goal

CodeCafe Next frontend must be designed for long-term scale.

It should support a future where the frontend becomes a large workspace product with multiple panels, editors, AI surfaces, collaboration flows, command systems, search, files, and complex domain interactions. The target is not a simple CRUD SPA. The architecture should leave room for a product that can grow toward Notion-like complexity without forcing a rewrite.

## Core Principles

- Organize by business capability, not by file type alone.
- Keep page files thin and move behavior into reusable slices.
- Separate server state, UI state, and editor state clearly.
- Build an application shell that can support multiple work surfaces.
- Prefer explicit boundaries over convenience imports.
- Treat the frontend as a product platform, not just a screen renderer for backend endpoints.

## Recommended Stack

- React
- TypeScript
- Vite
- React Router
- TanStack Query
- Zustand
- React Hook Form
- Zod
- Tailwind CSS with design tokens

This stack may evolve, but architecture decisions should not assume a tiny app.

## Frontend Shape

Use a Feature-Sliced Design inspired structure:

```text
src/
  app/
  processes/
  pages/
  widgets/
  features/
  entities/
  shared/
```

## Layer Responsibilities

### app

Owns:

- application bootstrap
- providers
- router
- query client setup
- global state composition
- theme and design system wiring
- global error boundaries

Must not own feature-specific business logic.

### processes

Owns long-running, cross-page product flows such as:

- workspace initialization
- auth/session restoration
- collaboration bootstrap
- realtime connection lifecycle

Use this layer sparingly. Not every workflow needs it.

### pages

Owns route-level composition.

Pages should:

- assemble widgets
- read route params
- define layout composition

Pages should not:

- contain complex orchestration logic
- call raw APIs directly
- hold deep domain logic

### widgets

Owns large UI blocks that compose features and entities.

Examples:

- sidebar
- page header
- editor surface
- AI panel
- inspector
- search panel

### features

Owns user-triggered capabilities and use cases.

Examples:

- create page
- rename note
- move block
- upload file
- run AI action
- resolve comment

Features should be as self-contained as possible and removable without breaking unrelated areas.

### entities

Owns core domain representations and related UI/state primitives.

Examples:

- workspace
- note
- page
- block
- conversation
- task
- file

Entities should contain stable domain-facing code, not arbitrary UI composition.

### shared

Owns foundational reusable code only:

- UI kit primitives
- API client
- utilities
- design tokens
- low-level hooks
- config
- generic helpers

Shared must not become a hidden business layer.

## Import Direction

Preferred dependency direction:

```text
app -> processes -> pages -> widgets -> features -> entities -> shared
```

Rules:

- Higher layers may import lower layers.
- Lower layers must not import higher layers.
- External code should use public exports, not deep internal imports.

## State Strategy

Frontend state must be split by purpose.

### Server State

Use TanStack Query for:

- API data
- cache
- pagination
- invalidation
- refetching
- optimistic updates where safe

Do not manually reimplement server-state management with `useEffect` and `useState`.

### Client UI State

Use local component state first.

Use Zustand for:

- sidebar open state
- command palette state
- inspector state
- workspace UI mode
- cross-screen UI state that truly needs sharing

Do not put all state into one global store.

### Editor and Complex Interaction State

The editor will likely become one of the most complex parts of the system.

Plan for:

- document model state
- selection state
- drag/drop state
- collaboration/presence state
- undo/redo behavior
- AI-assisted editing actions

Do not mix editor internals with ordinary page component state.

Editor state should be isolated behind dedicated modules and interfaces when it grows.

## App Shell Strategy

Design the frontend around a workspace shell, not around isolated pages.

The shell should be able to host:

- global navigation
- workspace sidebar
- content surface
- contextual right panel
- command palette
- AI assistant panel
- notifications
- modal layers

Even if the first version is simple, the structure should allow these surfaces to evolve independently.

## API Integration

- Never scatter raw `fetch` calls across pages and widgets.
- Centralize API access in shared client code and slice-level API functions.
- Let features and entities expose hooks or actions for the UI.
- Keep transport details out of page composition.

## Realtime Strategy

SignalR and realtime flows should be first-class frontend concerns.

Plan the architecture so the frontend can support:

- AI streaming responses
- task progress
- collaboration presence
- live updates to shared workspace data

Realtime connection management should live near app/process layers, not inside random page components.

## Performance Strategy

Assume the frontend will eventually contain:

- long lists
- nested trees
- rich editors
- multiple panels
- background polling or push updates

Therefore:

- keep state scoped tightly
- avoid giant top-level rerenders
- prepare for virtualization
- lazy load heavy surfaces
- isolate expensive widgets

Do not prematurely optimize every component, but do choose an architecture that allows optimization later.

## Styling Strategy

- Use a consistent design-token-driven styling system.
- Keep semantic tokens for colors, spacing, radius, typography, and depth.
- Avoid a large unstructured global CSS file.
- Keep visual primitives reusable and composable.

The UI does not need to be visually complete on day one, but the styling system should be coherent from day one.

## Testing Strategy

Frontend testing should cover:

- shared utilities
- slice-level logic
- feature interactions
- critical flows

Use:

- unit tests for pure logic
- integration tests for interactions
- e2e tests for important product paths

## Architectural Review Checklist

- Is the code in the correct FSD layer?
- Is this page doing too much?
- Is server state being managed manually?
- Is global state being introduced without clear need?
- Is business logic leaking into shared or app layers?
- Are imports respecting layer boundaries?
- Will this structure still hold if the editor and workspace grow 10x?
