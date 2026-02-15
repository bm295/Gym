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
cd BlazorApp
dotnet restore
dotnet run
```

Then open the URL shown in the terminal (typically `https://localhost:5001` or a nearby port), and click **Run demo** on the home page. You should see the final counter value (`400000`).

## Build

```bash
dotnet build ObjectHash.sln
```
