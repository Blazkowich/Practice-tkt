Patterns enforced throughout `EPM.TKT.Infrastructure/Repositories/`.

## Core rules

1. **Always `AsNoTracking()`** — every read query starts with `.AsNoTracking()`. No exceptions.
2. **Project inside the query** — never load an entity and map it afterwards. Use `.Select(e => new DomainModel { ... })` so SQL carries only the columns you need.
3. **Never `.Include()`** — navigate to related entities inside the `.Select()` projection:
   ```csharp
   Name = e.RelatedEntity != null ? e.RelatedEntity.Name : null
   ```
4. **`WhereIf` for optional filters** — from `EPM.TKT.Infrastructure.Extensions`:
   ```csharp
   .WhereIf(id.HasValue, e => e.RelatedId == id!.Value)
   .WhereIf(!string.IsNullOrWhiteSpace(search), e => e.Title != null && e.Title.Contains(search!))
   ```
   Avoid `if` blocks that reassign `query = query.Where(...)` when `WhereIf` is available.
5. **`EF.Functions.Like`** for case-insensitive string comparisons against a constant:
   ```csharp
   .Where(e => EF.Functions.Like(e.Name, "archived"))
   ```

## Pagination

```csharp
var total = await query.CountAsync(ct);
var items = await query
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .Select(...)
    .ToListAsync(ct);
return new PagedModel { Items = items, TotalCount = total };
```
Count the full query before paging; never count the paged result.

## Dynamic sorting

```csharp
var ordered = (sortField?.ToLowerInvariant(), sortDescending) switch
{
    ("title",     false) => query.OrderBy(e => e.Title),
    ("title",     true)  => query.OrderByDescending(e => e.Title),
    ("createdat", false) => query.OrderBy(e => e.CreatedAt),
    ("createdat", true)  => query.OrderByDescending(e => e.CreatedAt),
    (_,           true)  => query.OrderByDescending(e => e.CreatedAt),
    _                    => query.OrderBy(e => e.CreatedAt),
};
```

## Single-or-null records

```csharp
return await context.Entities
    .AsNoTracking()
    .Where(e => e.Id == id)
    .Select(e => new DomainModel { ... })
    .FirstOrDefaultAsync(ct);
```
Return `null` directly; the handler (or controller) decides whether to return 404.

## Aggregates

Prefer pushing aggregation into SQL via `.GroupBy(...).Select(g => new { g.Key, Count = g.Count() })` over materialising rows and grouping in memory. This is critical for large datasets.

## Two-query pattern

When a collection result requires data from a second table that cannot be efficiently joined (e.g. per-row lookup), fetch the primary collection first, extract the IDs, then fetch the secondary data in one batch query:

```csharp
var primary = await context.A.AsNoTracking().Where(...).Select(...).ToListAsync(ct);
var ids = primary.Select(x => x.Id).ToList();
var secondary = await context.B.AsNoTracking()
    .Where(b => ids.Contains(b.ForeignId))
    .Select(...)
    .ToListAsync(ct);
var map = secondary.GroupBy(b => b.ForeignId).ToDictionary(g => g.Key, g => g.First());
```
Never issue per-row queries inside a loop (N+1).

## DbContext injection

Repositories always receive `ITKTDbContext context` via primary constructor injection. Never depend on `TKTDbContext` directly — always use the interface.
