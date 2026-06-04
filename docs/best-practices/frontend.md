# Frontend Best Practices

## Philosophy

We optimize for:

- readability
- maintainability
- explicit state ownership
- scalable feature boundaries
- predictable UI behavior

We do not optimize for:

- clever abstractions
- premature optimization
- giant reusable components that try to do everything
- scattering API and state logic across random UI files

## Architectural Rules

- Follow the frontend structure defined in `docs/architecture/frontend.md`.
- Organize by business slices, not by flat technical folders only.
- Keep pages thin.
- Keep shared code truly generic.
- Respect layer import direction.

## File and Component Size

Rules of thumb:

- Be cautious when a component passes 100 lines.
- Strongly question components above 150 lines.
- If a `className` block becomes hard to scan, extract it.
- If a file mixes fetching, layout, forms, mutation logic, and modal orchestration, split it.

The goal is not tiny files. The goal is understandable files.

## Component Design

- One component should have one clear responsibility.
- Split layout composition from business interaction when useful.
- Avoid giant all-purpose components.
- Extract long inline event logic into named functions.

Bad smells:

- components doing fetch + mutation + modal state + layout + formatting
- JSX full of multi-branch business logic
- multiple unrelated responsibilities in one file

## State Rules

### Server State

Use TanStack Query for:

- fetching
- caching
- refetching
- invalidation
- stale and loading handling

If data comes from the backend, default to TanStack Query.

### Local UI State

Use `useState` first.

Examples:

- form field drafts
- toggle state
- small modal state
- local selection state

### Shared Client State

Use Zustand only when state truly needs to be shared or live beyond one component subtree.

Examples:

- workspace shell UI state
- command palette state
- global toasts
- inspector visibility

Do not move ordinary local component state into global stores.

## useEffect Rules

Do not use `useEffect` for:

- ordinary data fetching
- syncing derived state that can be computed directly
- patching around poor component structure

Use `useEffect` only for real side effects such as:

- subscriptions
- imperative bridge code
- timers
- browser APIs that require lifecycle handling

## API Rules

- Do not call `fetch` or `axios` directly inside page or widget components.
- Use a shared API client.
- Expose feature-level or entity-level hooks/actions for UI code.
- Keep transport details out of view composition.
- Mutation flows must refresh or replace affected cache entries predictably.

## FSD Import Rules

- Import across slices through public exports.
- Avoid deep imports into slice internals.
- Lower layers must not import higher layers.
- Do not put business logic into `shared`.

## Forms

For non-trivial forms, use:

- React Hook Form
- Zod

Expect this for:

- multiple fields
- validation rules
- mutation submission
- reusable form logic

## Async UI Rules

Every async screen or section should explicitly handle:

- loading
- error
- empty
- success

Do not assume API data always exists.

## Styling Rules

- Use a consistent styling strategy across the app.
- Prefer design tokens over raw semantic drift.
- Keep global CSS minimal.
- Use inline styles only for real runtime values that cannot be expressed cleanly otherwise.
- Avoid giant CSS dumping grounds.

## Accessibility Rules

- Use semantic HTML.
- Use real buttons for actions.
- Inputs need labels.
- Interactive areas must be keyboard-usable.
- Add aria attributes when semantics alone are not enough.

## Performance Rules

- Do not overuse `useMemo` and `useCallback`.
- Measure before optimizing.
- Lazy load heavy surfaces.
- Keep large state subscriptions narrow.
- Plan for virtualization in large lists and trees.
- Avoid top-level rerenders that repaint the whole workspace unnecessarily.

## Large Frontend Safety Rules

Because this frontend may become very large:

- do not centralize everything in `App.tsx`
- do not create one global mega store
- do not let page components become orchestration engines
- do not mix editor concerns with ordinary form/page concerns
- do not bypass slice boundaries for convenience

## Testing Rules

Must test:

- pure shared logic
- feature state logic
- API transformations
- important user interactions
- regressions for real bugs

Recommended tools:

- Vitest
- React Testing Library
- Playwright

## Review Checklist

- Is this component too large?
- Is the state in the right place?
- Is server state being handled with TanStack Query?
- Is `useEffect` being used for the wrong reason?
- Is the file mixing too many responsibilities?
- Is a deep import bypassing slice boundaries?
- Is business logic leaking into `pages`, `app`, or `shared`?
- Does async UI handle loading, error, empty, and success states?
- Are accessibility basics present?
- Will this still be maintainable when the workspace UI becomes much larger?
