# ObjectHash Blazor Demo (.NET 10 / C# 14)

This project has been updated to use **.NET 10** with **C# 14 (preview)** and the modern minimal hosting model.

## What this demonstrates

A thread-safe shared counter incremented from multiple parallel workers:

```csharp
private static readonly object _gate = new();
private static int _counter;

public static void Increment()
{
    for (int i = 0; i < 100_000; i++)
    {
        lock (_gate)
        {
            _counter++;
        }
    }
}

Parallel.Invoke(
    () => Increment(),
    () => Increment(),
    () => Increment(),
    () => Increment()
);

Console.WriteLine(_counter);
```

In the app, this logic is implemented in `ThreadSafeCounterDemo` and invoked from the home page.

## Prerequisites

- .NET 10 SDK Preview (or newer compatible preview)

## Run locally

From the repository root:

```bash
cd Gym.WebUI
dotnet restore
dotnet run
```

Then open the URL shown in the terminal (typically `https://localhost:5001` or a nearby port), and click **Run demo** on the home page. You should see the final counter value (`400000`).

## Build

```bash
dotnet build Gym.sln
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
`Jwt:SigningKey` through environment-specific configuration or environment variables before deployment.
