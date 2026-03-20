# PAM — Patient & Appointment Management

A full stack dental clinic app with .NET Core backend and React Typescript frontend.

## Stack

| Layer | Technologies |
|-------|-------------|
| **Backend** | .NET 10, ASP.NET Core, Entity Framework Core (In Memory), xUnit + Moq |
| **Frontend** | React 19, Typescript, React Router v7, Tanstack Query, React Hook Form, Zod, TailwindCSS, ShadCN UI, Vite 8 |
| **Infra** | Terraform, GitHub Actions, Docker, Azure Container Apps, Azure Container Registry, Azure Blob Storage |

## Run

```bash
cd backend && dotnet run          # API at localhost:5057
dotnet test backend.Tests         # run tests
cd frontend && pnpm install && pnpm dev   # UI at localhost:5173
```

Folder structure kept simple, yet effective for the scope of the app.

## Design Decisions

### Backend

- **Validate in data transformation layer.** Request DTOs handle validation, services only contain business logic.
- **Photo upload security.** Extension whitelist and magic byte signature to prevent spoofing. Filenames randomized on the server, clients can't see real file names.
- **Swappable storage.** Local disk for development, Azure Blob Storage for production.
- **Time range conflict detection.** Appointments store both start and end times, conflicts checked with the standard overlap formula directly on stored columns.
- **Seed data** on model creation, suitable for demo purposes.
- **In memory db**, swap to persistent db in production.
- **No SOLID for the sake of SOLID.** Added complexity should solve a real problem.

### Frontend

- **Tanstack Query for server state**, no Redux or Zustand. Server state is the only shared state in the app. It gives caching, retries, error and loading states out of the box.
- **Zod schemas that mirror the backend DTOs**, combined with React Hook Form for form level error handling.
- **Search lives in the URL**, not component state; shareable, survives refresh; debounced API calls with 400ms interval.
- **Frontend doesn't care which backend storage is active.** Abstraction on `getImageUrl`.
- **Dialogs reset on close.**

### Infrastructure

- **OIDC authentication** for GitHub Actions, no stored secrets.
- **Least privileges possible.**
- **Docker layer caching.**

## What I Would Do Different in Production

- Replace in memory db with persistent relational db, add migrations and health checks.
- Add authentication to protect the API; like Azure Entra ID or JWT.
- Use Azure Front Door or Static Web Apps to keep backend private instead of public and only protected with CORS policies.
- Make photo storage private, and serve through the backend. Critical for patients privacy.
- Add frontend tests for critical flows.
- Remote Terraform state instead of local, stored in Azure Storage.
- Database level policies on appointment conflicts.
