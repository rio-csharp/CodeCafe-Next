import { useCurrentWorkspace } from '@processes/workspace';

import './WorkspaceShellPage.css';

const navigationItems = [
  { label: 'Workspace', marker: 'W', active: true },
  { label: 'Notes', marker: 'N', active: false },
  { label: 'Code', marker: 'C', active: false },
  { label: 'AI Context', marker: 'A', active: false },
] as const;

const workspaceStats = [
  { label: 'Open lanes', value: '3' },
  { label: 'Active focus', value: 'Shell' },
  { label: 'Sync state', value: 'Ready' },
] as const;

export function WorkspaceShellPage() {
  const currentWorkspace = useCurrentWorkspace();
  const workspace = currentWorkspace.data;

  return (
    <div className="workspaceShell">
      <aside className="workspaceShell__sidebar" aria-label="Workspace navigation">
        <div className="workspaceShell__brand">
          <span className="workspaceShell__brandMark" aria-hidden="true">
            CC
          </span>
          <span className="workspaceShell__brandText">CodeCafe</span>
        </div>

        <nav className="workspaceShell__nav" aria-label="Primary">
          {navigationItems.map((item) => (
            <button
              className="workspaceShell__navItem"
              data-active={item.active}
              disabled={!item.active}
              key={item.label}
              type="button"
            >
              <span className="workspaceShell__navMarker" aria-hidden="true">
                {item.marker}
              </span>
              <span>{item.label}</span>
            </button>
          ))}
        </nav>
      </aside>

      <main className="workspaceShell__main">
        <header className="workspaceShell__topbar">
          <div>
            <p className="workspaceShell__eyebrow">Current workspace</p>
            <h1>{workspace?.name ?? 'Loading workspace'}</h1>
          </div>
          <div className="workspaceShell__workspaceBadge" aria-live="polite">
            <span className="workspaceShell__statusDot" data-status={workspace?.status ?? 'syncing'} />
            <span>{workspace?.handle ? `@${workspace.handle}` : 'connecting'}</span>
          </div>
        </header>

        <section className="workspaceShell__surface" aria-label="Workspace surface">
          <div className="workspaceShell__surfaceHeader">
            <div>
              <p className="workspaceShell__eyebrow">Workspace surface</p>
              <h2>Today</h2>
            </div>
            <button className="workspaceShell__quietButton" type="button">
              Focus
            </button>
          </div>

          <div className="workspaceShell__statGrid" aria-label="Workspace summary">
            {workspaceStats.map((stat) => (
              <div className="workspaceShell__stat" key={stat.label}>
                <span>{stat.label}</span>
                <strong>{stat.value}</strong>
              </div>
            ))}
          </div>

          <div className="workspaceShell__canvas">
            <div className="workspaceShell__lane">
              <span className="workspaceShell__laneKicker">Entry</span>
              <h3>Workspace home</h3>
              <p>Workspace activity, focus lanes, and shared context collect here.</p>
            </div>
            <div className="workspaceShell__lane workspaceShell__lane--muted">
              <span className="workspaceShell__laneKicker">Next</span>
              <h3>Notes</h3>
              <p>No notes are open.</p>
            </div>
            <div className="workspaceShell__lane workspaceShell__lane--muted">
              <span className="workspaceShell__laneKicker">Next</span>
              <h3>Code</h3>
              <p>No code sessions are open.</p>
            </div>
          </div>
        </section>
      </main>

      <aside className="workspaceShell__contextPanel" aria-label="AI context panel">
        <div className="workspaceShell__contextHeader">
          <p className="workspaceShell__eyebrow">Context</p>
          <h2>AI Panel</h2>
        </div>
        <div className="workspaceShell__contextBody">
          <div className="workspaceShell__contextRow">
            <span>Workspace</span>
            <strong>{workspace?.name ?? 'Loading'}</strong>
          </div>
          <div className="workspaceShell__contextRow">
            <span>Mode</span>
            <strong>Shell preview</strong>
          </div>
          <div className="workspaceShell__contextRow">
            <span>Status</span>
            <strong>{currentWorkspace.isError ? 'Unavailable' : 'Ready'}</strong>
          </div>
        </div>
      </aside>
    </div>
  );
}
