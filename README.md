# CodeCafe

> A modular .NET 10 backend with a Vite + React + TypeScript frontend, organized around Domain-Driven Design and clean architecture.

## 🏗️ Architecture Overview

CodeCafe follows a **modular monolith** pattern, organized into clear bounded contexts with explicit layering. The backend is built on .NET 10 and uses ASP.NET Core for the host and adapters. The frontend is a modern Vite-powered SPA with Feature-Sliced Design (FSD) layout.

### Backend Layers

- **BuildingBlocks** — Cross-cutting infrastructure (SharedKernel entities/value objects, persistence, execution, observability, DI helpers)
- **Modules** — Each module (`Platform`, `Notes`, `Code`, `AI`) has its own four-layer split:
  - `Domain` — entities, value objects, domain events
  - `Application` — use cases, command/query handlers
  - `Infrastructure` — EF Core, external service adapters
  - `Contracts` — DTOs and integration events exposed to other modules. Depends only on `BuildingBlocks` / `SharedKernel`, never on sibling layers inside the same module.
- **Adapters** — Inbound transports:
  - `Web` — REST/Controller API
  - `Mcp` — Model Context Protocol server
  - `Realtime` — SignalR hubs
- **Host** — Composition root that wires up adapters, modules, and cross-cutting concerns

### Frontend (codecafe-web)

Uses the **Feature-Sliced Design** convention:

- `app/` — bootstrap, providers, router, global store
- `pages/` — route-level components
- `widgets/` — composite UI blocks
- `features/` — user-facing actions
- `entities/` — business entities and their UI
- `shared/` — reusable utilities and UI primitives
- `processes/` — multi-step business processes

Frontend stack: **Vite 5 + React 18 + TypeScript 5**, with **TanStack Query** for server state, **Zustand** for opt-in UI state, **react-hook-form + zod** for forms, and **react-router** for routing. Path aliases (`@app/*`, `@pages/*`, `@features/*`, …) are wired in both `tsconfig.json` and `vite.config.ts`.

## 📁 Project Structure

```
CodeCafe-Next/
├── CodeCafe.slnx              # XML solution file (slnx format, default in .NET 9+)
├── .gitignore
├── README.md
├── CONTRIBUTING.md
├── src/
│   ├── BuildingBlocks/        # CodeCafe.SharedKernel, CodeCafe.BuildingBlocks
│   ├── Modules/               # Bounded contexts (Platform, Notes, Code, AI)
│   │                          # each with Domain / Application / Infrastructure / Contracts
│   ├── Adapters/              # Inbound transports (Web, Mcp, Realtime)
│   ├── Host/                  # CodeCafe.Host (composition root)
│   └── Frontend/codecafe-web/ # Vite + React + TS
├── tests/
│   ├── Backend/               # ArchitectureTests, IntegrationTests, UnitTests
│   └── Frontend/              # unit, integration, e2e
├── docs/
│   ├── architecture/          # backend.md, frontend.md
│   └── api/
└── scripts/                   # Operational scripts
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- npm / pnpm / yarn

### Backend

```bash
# Restore and build (slnx is the default solution format in .NET 9+)
dotnet restore CodeCafe.slnx
dotnet build CodeCafe.slnx

# Run the host
dotnet run --project src/Host/CodeCafe.Host
```

### Frontend

```bash
cd src/Frontend/codecafe-web
npm install
npm run dev          # http://localhost:5173
npm run build        # tsc -b && vite build
```

## 🧪 Testing

```bash
# Backend — use the .csproj path so dotnet test is unambiguous
dotnet test tests/Backend/UnitTests/CodeCafe.UnitTests.csproj
dotnet test tests/Backend/IntegrationTests/CodeCafe.IntegrationTests.csproj
dotnet test tests/Backend/ArchitectureTests/CodeCafe.ArchitectureTests.csproj

# Frontend (run from src/Frontend/codecafe-web)
cd src/Frontend/codecafe-web
npm run type-check   # tsc --noEmit
npm run test         # vitest run (all)
npm run test:unit    # tests/Frontend/unit
npm run test:integration  # tests/Frontend/integration
npm run test:e2e     # playwright
```

## 📚 Documentation

- [Backend Architecture](docs/architecture/backend.md)
- [Frontend Architecture](docs/architecture/frontend.md)
- [API Reference](docs/api/)

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

TBD
