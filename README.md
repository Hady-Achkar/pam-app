# PAM - Patient & Appointment Management

A full stack dental clinic app with .NET Core backend and React Typescript frontend.

## Stack

Backend: .NET 10, ASP.NET Core, Entity Framework Core (In Memory), xUnit + Moq

Frontend: React 19, Typescript, React Router v7, Tanstack Query, React Hook Form, Zod, TailwindCSS, ShadCN UI, Vite 8

Infra: Terraform, GitHub Actions, Docker, Azure Container Apps, Azure Container Registry (DockerHub equivilant), Azure Blob Storage

## Run

```bash
cd backend && dotnet run
dotnet test backend.Tests
cd frontend && pnpm install && pnpm dev
```

Folder structure kept simple, yet effective for the scope of the app.

## Design Decisions

### Backend

- Validate in data transformation layer.
- Photo upload security: files are validated by extension whitelist and magic byte signature to prevent spoofing (e.g do it). Filenames randomized on the server, clients can't see real file names.
- Swappable storage: local disk for development, azure blob storage for production.
- Time range conflict detection: short meaningful description
- Seed data on model creation, suitable for demo purposes.
- In memory db, change to persistent db in production
- No solid for the sake of solid. Added complexity should solve a real problem.

### Frontend

- Tanstack Query for server state, no redux or zustand. Server state is the only shared state in the app. It gives caching, retries, error and loading states out of the box.
- Zod schemas that mirror the backend DTOs, combined with React Hook Form for form level error handling.
- Search lives in the url, not component state; shareable, survives refresh; debounced api calls with 400ms interval.
- Frontend doesn't care which backend storage is active. Abstraction on getImageUrl
- Dialogues reset on close

### Infrastructure

- OIDC authentication for github actions, no stored secrets.
- Least privelleges possible.
- Docker caching

## What I Would Do Different in Production

- Replace in memory db with persistent relational db, add migrations and healthchecks.
- Add authentication to protect the api; like Azure Entra ID or jwt.
- Use Azure front door or Static web apps to keep backend private instead of public and only protected with CORS policies.
- Make photo storage private, and serve through the backend. Critical for patients privacy
- Add frontend tests for critical flows.
- Remote terraform state instead of local, stored in azure storage for state.
- Database level policies on appointments conflicts.
