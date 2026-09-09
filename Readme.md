# Gym SaaS (.NET 10 / C# 14)

Gym management application for members, subscriptions, payments, branches, and attendance.

## Prerequisites

- .NET 10 SDK Preview (or newer compatible preview)

## Run locally

From the repository root, run the UI and API hosts separately:

```bash
cd Gym.WebUI
dotnet restore
dotnet run
```

```bash
cd Gym.Api
dotnet restore
dotnet run
```

Each host listens on the URL shown by its process.

## Build

```bash
dotnet build Gym.sln
```

## Test

```bash
dotnet test Gym.sln
```

## Payment void authorization

The payment void endpoint demonstrates RBAC together with tenant and branch scoping:

```text
POST /api/tenants/{tenantId}/branches/{branchId}/payments/{paymentId}/void
Authorization: Bearer <JWT>
Content-Type: application/json

{ "reason": "Duplicate payment" }
```

Only the `TenantAdmin` and `BranchManager` roles satisfy the `CanVoidPayment` policy. The JWT must also
contain a `tenant_id` claim matching the route. A `BranchManager` needs a matching `branch_id` claim;
`TenantAdmin` can access every branch within the tenant. Configure `Jwt:Issuer`, `Jwt:Audience`, and
`Jwt:SigningKey` for `Gym.Api` through environment-specific configuration or environment variables before deployment.
