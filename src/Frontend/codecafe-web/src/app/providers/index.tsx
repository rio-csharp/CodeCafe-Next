import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useState, type ReactNode } from 'react';

interface AppProvidersProps {
  children: ReactNode;
}

/**
 * Root provider tree. Currently hosts the TanStack Query client.
 *
 * Rules of thumb:
 * - Server state belongs in TanStack Query.
 * - Cross-screen UI state should be added as a small, focused Zustand store
 *   inside the feature that owns it. Do NOT create a global mega store here.
 * - Keep this file boring: only providers, no business logic.
 */
export function AppProviders({ children }: AppProvidersProps) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            staleTime: 30_000,
            refetchOnWindowFocus: false,
            retry: 1,
          },
        },
      }),
  );

  return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
}
