import { toWorkspaceId, type CurrentWorkspaceResponse } from './types';

export interface WorkspaceClient {
  getCurrentWorkspace(): Promise<CurrentWorkspaceResponse>;
}

const placeholderCurrentWorkspace: CurrentWorkspaceResponse = {
  id: toWorkspaceId('local-workspace'),
  name: 'CodeCafe Workspace',
  handle: 'codecafe',
  status: 'active',
};

export const workspaceClient: WorkspaceClient = {
  getCurrentWorkspace() {
    return Promise.resolve(placeholderCurrentWorkspace);
  },
};
