Adding a repository spans three files. Do all three before finishing.

## 1. Interface — `EPM.TKT.App/Repositories/I[Domain]Repository.cs`

```csharp
using EPM.TKT.Domain.Enums;
using EPM.TKT.Domain.Models.[Domain];

namespace EPM.TKT.App.Repositories;

public interface I[Domain]Repository
{
    Task<[ReturnType]?> Get[Something]Async(
        [parameters],
        CancellationToken cancellationToken = default);
}
```

- All methods are `Task<T>` async.
- `CancellationToken cancellationToken = default` is the last parameter on every method.
- Return nullable `T?` when a single record may not exist; use `IReadOnlyList<T>` for collections.
- Methods returning a collection never return null — return an empty list instead.

## 2. Implementation — `EPM.TKT.Infrastructure/Repositories/[Domain]Repository.cs`

```csharp
using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.[Domain];
using EPM.TKT.Infrastructure.Entities.TKT;
using EPM.TKT.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal class [Domain]Repository(ITKTDbContext context) : I[Domain]Repository
{
    public async Task<[ReturnType]?> Get[Something]Async(
        int id,
        CancellationToken cancellationToken = default)
        => await context.[DbSet]
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new [ReturnType]
            {
                Id   = e.Id,
                Name = e.Name,
            })
            .FirstOrDefaultAsync(cancellationToken);
}
```

Key rules:
- Class is `internal` (not public).
- Primary constructor — inject `ITKTDbContext context`.
- **Always** call `.AsNoTracking()` on every read query.
- **Never** use `.Include()` — navigate to related entities inside `.Select()` projections instead: `Name = e.NavProp != null ? e.NavProp.Name : null`.
- Use the `WhereIf(bool condition, Expression<Func<T,bool>> predicate)` extension for optional filters (from `EPM.TKT.Infrastructure.Extensions`).
- Project directly to domain model inside the LINQ query; do not materialise entities and map afterwards.
- For paginated queries: `Skip((page - 1) * size).Take(size)` after applying all filters.
- Dynamic sort: use a `switch` expression on the sort-field string, with a default fallback.

## 3. DI registration — `EPM.TKT.Infrastructure/ServiceCollectionExtensions.cs`

Add to the chain in `AddInfrastructure`:

```csharp
.AddScoped<I[Domain]Repository, [Domain]Repository>()
```

Use `.AddScoped` for all repositories — they should live for the lifetime of a single HTTP request and share the EF Core `DbContext` instance.

## Namespace reference

| Location | Namespace |
|---|---|
| Domain models | `EPM.TKT.Domain.Models.[Domain]` |
| Domain enums | `EPM.TKT.Domain.Enums` |
| Entity classes | `EPM.TKT.Infrastructure.Entities.TKT` |
| Repository interfaces | `EPM.TKT.App.Repositories` |
| Repository implementations | `EPM.TKT.Infrastructure.Repositories` |
