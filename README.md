# 🛒 CartMate

A shared, real-time grocery list for two. One partner adds items at home; the other checks
them off in the store and gets notified about anything new.

[![CI](https://github.com/javedkhan2k2/cartmate/actions/workflows/ci.yml/badge.svg)](https://github.com/javedkhan2k2/cartmate/actions/workflows/ci.yml)
[![Security](https://github.com/javedkhan2k2/cartmate/actions/workflows/security.yml/badge.svg)](https://github.com/javedkhan2k2/cartmate/actions/workflows/security.yml)
[![Deploy staging](https://github.com/javedkhan2k2/cartmate/actions/workflows/cd-staging.yml/badge.svg)](https://github.com/javedkhan2k2/cartmate/actions/workflows/cd-staging.yml)

## Tech stack

| Layer    | Technology |
|----------|------------|
| API      | .NET 10, FastEndpoints, Mediator (CQRS), FluentValidation, EF Core, SignalR |
| Frontend | Blazor WebAssembly |
| Database | PostgreSQL (Neon in the cloud, Docker locally) |
| Hosting  | Render (API), Azure Static Web Apps (frontend), GHCR (images) |
| CI/CD    | GitHub Actions |

## Getting started

Prerequisites: .NET 10 SDK, Docker.

```bash
git clone https://github.com/javedkhan2k2/cartmate.git
cd cartmate
dotnet tool restore
docker compose up -d db          # local Postgres
dotnet run --project src/CartMate.Api
dotnet run --project src/CartMate.Web
```

## Running tests

```bash
dotnet test                      # unit, integration (needs Docker), component, architecture
```

## Contributing

1. Branch from `main`: `feature/<short-description>` or `fix/<short-description>`.
2. Use [Conventional Commits](https://www.conventionalcommits.org): `feat: add item categories`.
3. Open a pull request. All checks must pass and one approval is required.
4. Squash-merge. Staging deploys automatically.

## Environments

| Environment | Frontend | API |
|-------------|----------|-----|
| Staging     | _added in Chapter 21_ | _added in Chapter 21_ |
| Production  | _added in Chapter 21_ | _added in Chapter 21_ |
