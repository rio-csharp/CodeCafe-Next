export type WorkspaceId = string & { readonly __brand: 'WorkspaceId' };

export interface CurrentWorkspaceResponse {
  id: WorkspaceId;
  name: string;
  handle: string;
  status: 'active' | 'offline' | 'syncing';
}

export function toWorkspaceId(value: string): WorkspaceId {
  return value as WorkspaceId;
}
