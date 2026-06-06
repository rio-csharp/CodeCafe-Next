import { createBrowserRouter } from 'react-router-dom';

import { WorkspaceShellPage } from '@pages/workspace-shell';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <WorkspaceShellPage />,
  },
]);
