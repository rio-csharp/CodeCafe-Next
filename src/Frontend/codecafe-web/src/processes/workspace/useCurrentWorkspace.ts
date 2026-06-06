import { useQuery } from '@tanstack/react-query';

import { workspaceClient } from './workspaceClient';

export const currentWorkspaceQueryKey = ['workspace', 'current'] as const;

export function useCurrentWorkspace() {
  return useQuery({
    queryKey: currentWorkspaceQueryKey,
    queryFn: () => workspaceClient.getCurrentWorkspace(),
  });
}
