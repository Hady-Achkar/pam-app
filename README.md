# PAM - Patient & Appointment Management

A full-stack dental clinic application. ASP.NET Core 10 backend with Entity Framework Core, React 19 frontend with TypeScript. Manages patients, appointments, dentists, and treatments with photo upload support and swappable cloud storage.

## Tech Stack

### Backend

- .NET 10 / ASP.NET Core
- Entity Framework Core (InMemory provider — swap to SQL Server/PostgreSQL for production)
- Azure Blob Storage SDK (with local disk fallback for development)
- xUnit + Moq for testing

### Frontend

- React 19 / TypeScript
- React Router v7
- TanStack React Query
- React Hook Form + Zod
- Tailwind CSS v4 + shadcn/ui (Radix primitives)
- Axios
- Vite 8

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org)
- [pnpm](https://pnpm.io)

### Run

```bash
# backend
cd backend
dotnet run
```

API available at `http://localhost:5057`. Swagger UI at `http://localhost:5057/swagger`.

```bash
# frontend
cd frontend
pnpm install
pnpm dev
```

Frontend available at `http://localhost:5173`.

### Test

```bash
dotnet test backend.Tests
```

## API Endpoints

### Patients

| Method | Route                   | Description                            |
| ------ | ----------------------- | -------------------------------------- |
| `GET`  | `/api/patients?search=` | List patients (with optional search)   |
| `GET`  | `/api/patients/{id}`    | Get patient with appointments          |
| `POST` | `/api/patients`         | Create patient (`multipart/form-data`) |

### Appointments

| Method | Route                           | Description                     |
| ------ | ------------------------------- | ------------------------------- |
| `GET`  | `/api/appointments/{patientId}` | List appointments for a patient |
| `POST` | `/api/appointments`             | Book an appointment             |

### Lookup Data

| Method | Route             | Description         |
| ------ | ----------------- | ------------------- |
| `GET`  | `/api/dentists`   | List all dentists   |
| `GET`  | `/api/treatments` | List all treatments |

### Photos

| Method | Route                    | Description           |
| ------ | ------------------------ | --------------------- |
| `GET`  | `/api/photos/{fileName}` | Serve a patient photo |

## Project Structure

```
backend/
  Controllers/       API endpoints
  DTOs/              Request/response models
  Entities/          EF Core domain models
  Repositories/      Data access layer
  Services/          Business logic
  Storage/           Photo storage abstraction (local + Azure Blob)
  Mappings/          Entity-to-DTO extension methods
  ExceptionHandlers/ Global error handling (IExceptionHandler)
  Data/              DbContext + seed data

frontend/src/
  api/               Axios instance + API functions
  hooks/             React Query hooks (data fetching, mutations)
  pages/             Route-level components
  components/        Feature components + shadcn/ui primitives
  schemas/           Zod validation schemas
  types/             TypeScript interfaces (mirrors backend DTOs)
  lib/               Utilities (cn, date formatting)
```

## Backend Design Decisions

**Validation lives in DTOs.** Request DTOs implement `IValidatableObject` for input validation (trimming, length, future dates). The `[ApiController]` attribute auto-validates and returns 400 before services are called. Services only contain business logic (existence checks, photo processing).

**Photo upload security.** Files are validated by extension whitelist (.jpg, .png, .webp) and magic byte signature verification to prevent spoofing. Filenames are randomized server-side. Uploads are stored outside `wwwroot` and served through a dedicated controller to avoid direct file access. Max upload size enforced at both controller (`[RequestSizeLimit]`) and service level.

**Swappable storage.** Photo storage is abstracted behind `IPhotoStorage`. The active implementation is configured via `appsettings.json`:

```json
{
  "Storage": {
    "Provider": "Local"
  }
}
```

For Azure Blob Storage:

```json
{
  "Storage": {
    "Provider": "AzureBlob",
    "AzureBlob": {
      "ServiceUri": "https://youraccount.blob.core.windows.net"
    }
  }
}
```

Uses `ManagedIdentityCredential` in production — the Container App's system-assigned identity authenticates to Blob Storage with a custom RBAC role (read + write only, no delete) scoped to the photos container.

**Error handling.** A global `IExceptionHandler` maps service exceptions to HTTP responses using `ProblemDetails` (RFC 7807). `KeyNotFoundException` becomes 404, `ArgumentException` becomes 400, and 500s hide internal details.

**Seed data.** Dentists and treatments are seeded via EF Core's `OnModelCreating`. The InMemory provider resets on restart — suitable for development and demos.

## Frontend Design Decisions

**React Query over Redux/Zustand.** Server state is the only shared state in this app. React Query handles caching, background refetching, and loading/error states out of the box. No global state management needed — local component state handles UI concerns like dialog open/close and form inputs.

**Validation mirrors the backend.** Zod schemas enforce the same constraints as backend DTOs (name length, file size/type, future dates). Invalid input is caught before it hits the network. React Hook Form integrates via `zodResolver` for field-level error messages.

**URL-synced search.** The search term lives in `useSearchParams`, not component state. This makes search results bookmarkable, shareable, and survive page refresh. API calls are debounced (400ms) while the input stays responsive on every keystroke.

**Lookup data caching.** Treatments use `staleTime: Infinity` since they're static seed data that never changes at runtime. The global default is 30s with a single retry — enough to avoid redundant fetches without serving stale dynamic data.

**Photo URL abstraction.** `getImageUrl` handles both relative paths from local storage (`/api/photos/file.jpg`) and absolute URLs from Azure Blob Storage (`https://...blob.core.windows.net/...`). The frontend doesn't need to know which storage backend is active.

**Error boundary.** A class component (still the only way in React 19) wraps the app to catch render crashes and show a reload fallback instead of a white screen.

**Form pattern.** Dialogs reset form state on close via `onOpenChange`. Mutations invalidate related query keys to keep the cache consistent — creating an appointment invalidates both the patient detail and the patients list so the appointment count badge stays accurate.

## Deployment

Deployed to Azure Container Apps via Terraform + GitHub Actions.

### Architecture

```
[pam-ca-frontend] → browser → CORS → [pam-ca-backend] → Azure Blob Storage
```

Both Container Apps are external with CORS. The backend authenticates to Blob Storage via managed identity with a custom authorization role (read + write, scoped to photos container only). Consumption plan scales to 0 replicas when idle.

### Infrastructure (Terraform)

```bash
cd terraform
terraform init
terraform plan
terraform apply
```

Resources: Resource Group, Container Apps Environment, ACR (Basic), Storage Account (LRS), Log Analytics, two Container Apps, custom RBAC role + assignment.

### CI/CD (GitHub Actions)

Push to `main` triggers `.github/workflows/deploy.yml`:

1. Builds backend + frontend Docker images
2. Pushes to ACR (tagged with commit SHA)
3. Deploys to Container Apps

Required GitHub Secrets: `AZURE_CREDENTIALS`, `ACR_LOGIN_SERVER`, `BACKEND_URL`.

### Production Tradeoffs

This setup exposes the backend URL publicly (CORS restricts which origins can call it). In production you would:

- **Azure Static Web Apps** — proxy `/api/*` to an internal backend via linked backend. SWA adds an identity provider that only accepts proxied requests.
- **Azure Front Door** — L7 routing with WAF and DDoS protection. Backend stays internal.
- **Authentication** — add Azure Entra ID to protect the API regardless of architecture.

## Assumptions

- No authentication/authorization — the scope is focused on CRUD and file uploads
- InMemory database — production would use a persistent provider
- Photo storage defaults to local disk — production uses Azure Blob Storage with managed identity
- Both Container Apps are external with CORS — production would Front Door to keep the backend internal
- Dentists and treatments are static seed data, not managed through the API
- No frontend tests — scope focused on architecture and patterns
