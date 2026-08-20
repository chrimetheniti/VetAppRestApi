# 🐾 VetApp — Veterinary Clinic Management System

A full-stack web application for managing a veterinary clinic:
patient (animal) profiles, owners, veterinarians and their accounts,
with role-based access control for Admin, Receptionist, Veterinarian
and Owner roles.

Final project for the **Coding Factory 10** program at the Athens
University of Economics and Business (AUEB).

---

## 📋 Table of contents

- [Tech stack](#-tech-stack)
- [Architecture](#-architecture)
- [Features](#-features)
- [Prerequisites](#-prerequisites)
- [Setup & run (local development)](#-setup--run-local-development)
- [Setup & run (Docker)](#-setup--run-docker)
- [Seed data & test credentials](#-seed-data--test-credentials)
- [API documentation](#-api-documentation)
- [Project structure](#-project-structure)

---

## 🛠 Tech stack

### Backend
- **.NET 10** Web API (C#)
- **Entity Framework Core** (Code First, migrations)
- **SQL Server 2022** (Docker)
- **AutoMapper** — DTO ↔ entity mapping
- **BCrypt.Net** — password hashing
- **JWT Bearer** — authentication
- **Serilog** — structured logging
- **Swashbuckle** — OpenAPI / Swagger

### Frontend
- **React 19** + **TypeScript**
- **Vite** — dev server & build
- **Tailwind CSS v4** + **shadcn/ui** (Radix primitives)
- **React Router 7** — routing
- **React Hook Form** + **Zod** — form state & validation
- **Sonner** — toast notifications
- **Lucide** — icons

---

## 🏗 Architecture

The backend follows a **layered architecture** (Domain-Driven Design):

```
Controllers  →  Services  →  Repositories  →  DbContext  →  SQL Server
      ↑
   DTOs (input/output)  ← AutoMapper →  Domain entities
```

- **Controllers** — HTTP boundary, no business logic
- **Services** — orchestration, cross-repository logic
- **Repositories** — data access, one per aggregate root
- **DTOs** — separate shapes for signup / read / update
- **JWT-based authentication** with role and capability claims
- **Global exception handler** normalizes all errors to a consistent JSON shape

The frontend consumes the REST API via typed `fetch` wrappers and stores
the JWT in an HTTP-only cookie. Routes are protected by an
`AuthProvider` context and a `ProtectedRoute` component. Role-based
access is enforced both server-side (authorization policies) and
client-side (conditional rendering).

---

## ✨ Features

- 🔐 **Authentication & authorization** — JWT + role-based policies (Admin, Receptionist, Veterinarian, Owner)
- 🐕 **Patient CRUD** — animal profiles with species, breed, chip number, date of birth, owner and assigned veterinarian
- 👤 **Owner CRUD** — customer accounts
- 🩺 **Veterinarian CRUD** — clinic staff accounts
- 📝 **Public owner self-registration**
- 👋 **Role-aware dashboard** — quick actions and stats per role
- ✏️ **My Profile** — vets and owners can update their own details
- 📄 **Pagination** on all list views
- 🎨 **Responsive UI** matching a custom design system (deep teal palette)

---

## ✅ Prerequisites

- **.NET SDK 10.0** — https://dotnet.microsoft.com/download
- **Node.js 20+** and **npm** — https://nodejs.org
- **Docker Desktop** — https://docker.com/products/docker-desktop
- **A SQL client** (optional but useful) — DBeaver, Azure Data Studio, or SSMS

---

## 🚀 Setup & run (local development)

This is the recommended flow: the database runs in Docker, the backend
and frontend run natively on your machine.

### 1. Clone the repository

```bash
git clone https://github.com/chrimetheniti/VetAppRestApi.git
cd VetAppRestApi
```

### 2. Configure environment variables

Copy the template and fill in the values:

```bash
cp .env.example .env
```

The important ones:
- `SA_PASSWORD` — the SQL Server `sa` password (used by Docker)
- `JWT_SECRET` — a base64 string of at least 32 bytes; generate one with:
  ```bash
  openssl rand -base64 32
  ```

### 3. Start the database

```bash
docker-compose up -d sqlserver
```

Wait ~15 seconds for SQL Server to accept connections. Verify:

```bash
docker ps
```

You should see the `vetrestapi-db` container listed as healthy.

### 4. Apply database migrations

From the repository root:

```bash
cd VetApp
dotnet ef database update
```

This creates the schema and seeds core reference data (roles,
capabilities, role-capability mappings).

### 5. Load test data

Import `VetDB_TestData_Seed.sql` into the `VetRestApiDB` database
(via DBeaver, Azure Data Studio, or `sqlcmd`). This adds sample
veterinarians, owners, and patients so you can log in and try the app.

Example with `sqlcmd`:

```bash
sqlcmd -S localhost,1437 -U sa -P "<your-sa-password>" -d VetRestApiDB -i VetDB_TestData_Seed.sql
```

### 6. Run the backend

Still inside the `VetApp/` folder:

```bash
dotnet run
```

The API starts at `https://localhost:7213` (HTTPS) and
`http://localhost:5273` (HTTP).

Swagger UI is available at `https://localhost:7213/swagger`.

### 7. Run the frontend

Open a **second terminal**:

```bash
cd vetapp-client
cp .env.example .env    # if not already present
npm install
npm run dev
```

The frontend starts at `http://localhost:5173`. Open it in a browser
and log in with one of the test credentials below.

---

## 🐳 Setup & run (Docker)

You can also run the backend inside Docker alongside the database.
Uncomment the `webapp` service in `docker-compose.yml` and run:

```bash
docker-compose up -d
```

The backend will be available at `http://localhost:8081`. The frontend
still runs locally as described above, but change its `VITE_API_URL`
in `vetapp-client/.env` to:

```
VITE_API_URL=http://localhost:8081/api/v1
```

---

## 🌱 Seed data & test credentials

After importing `VetDB_TestData_Seed.sql`, the following accounts are
available (all with password **`Test123!`**):

| Role          | Username            | Notes                       |
| ------------- | ------------------- | --------------------------- |
| Admin         | `admin`             | Full access                 |
| Veterinarian  | `vet_papadopoulos`  | Sees patients, own profile  |
| Owner         | `test_owner_1`      | Sees own profile            |

The seed includes **5 veterinarians**, **15 owners**, and **30 patients**
for realistic testing.

---

## 📚 API documentation

Interactive Swagger UI is available at:

```
https://localhost:7213/swagger
```

All endpoints are documented with request/response schemas. Protected
endpoints require a JWT token — obtain one via `POST /api/v1/auth/login`
and click the **Authorize** button in Swagger to attach it.

---

## 📁 Project structure

```
VetAppRestApi/
├── VetApp/                    # Backend (.NET 10 Web API)
│   ├── Controllers/           # HTTP endpoints
│   ├── Services/              # Business logic
│   ├── Repositories/          # Data access
│   ├── Models/                # Domain entities
│   ├── DTO/                   # Request/response DTOs
│   ├── Data/                  # DbContext, migrations
│   ├── Security/              # JWT, encryption, claims
│   ├── Configuration/         # AutoMapper profiles
│   ├── Exceptions/            # Domain exceptions
│   └── Helpers/               # Cross-cutting utilities
├── vetapp-client/             # Frontend (React + Vite)
│   ├── src/
│   │   ├── api/               # Typed fetch wrappers
│   │   ├── components/        # Shared UI + shadcn/ui
│   │   ├── context/           # Auth context
│   │   ├── pages/             # Route screens
│   │   ├── schemas/           # Zod schemas + TS types
│   │   └── utils/             # Cookie helpers
│   └── public/                # Static assets (logo)
├── docker-compose.yml         # Database + optional backend
├── Dockerfile                 # Backend image
├── .env.example               # Backend env template
└── VetDB_TestData_Seed.sql    # Sample data
```

---

## 👤 Author

**Christina Metheniti**
Coding Factory 10 — Athens University of Economics and Business
GitHub: [@chrimetheniti](https://github.com/chrimetheniti)
