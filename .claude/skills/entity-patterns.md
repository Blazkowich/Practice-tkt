Adding a new EF Core entity requires changes in three places.

## 1. Entity class — `EPM.TKT.Infrastructure/Entities/TKT/[Name].cs`

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.TKT;

internal class [Name]
{
    [Key]
    public int [Name]Id { get; set; }

    [MaxLength(500)]
    public string? Title { get; set; }

    public bool? IsActive { get; set; }

    // FK scalar
    public int? RelatedId { get; set; }

    // Nav property
    [ForeignKey(nameof(RelatedId))]
    public RelatedEntity? Related { get; set; }
}
```

Rules:
- Class is `internal`.
- PK is named `[ClassName]Id` and carries `[Key]`.
- String columns always have `[MaxLength(N)]` and are nullable (`string?`) unless the schema enforces NOT NULL.
- FK scalars are nullable (`int?`) by default.
- Navigation properties are decorated with `[ForeignKey(nameof(ScalarFkProperty))]`.
- Use `[Column("ExactSqlColumnName")]` when the SQL column name differs from the property name.
- No `virtual` keyword — lazy loading is not used.
- No `ICollection<>` inverse navigations unless the query actually needs them.

## 2. `ITKTDbContext` interface — `EPM.TKT.Infrastructure/ITKTDbContext.cs`

Add a `DbSet<[Name]>` property:

```csharp
DbSet<[Name]> [PluralName] { get; set; }
```

## 3. `TKTDbContext` — `EPM.TKT.Infrastructure/TKTDbContext.cs`

Add the same `DbSet` property implementation:

```csharp
public DbSet<[Name]> [PluralName] { get; set; }
```

`OnModelCreating` in `TKTDbContext` applies any additional Fluent API configuration. Only add to it when data-annotation attributes are insufficient (e.g. composite keys, owned types, table-per-hierarchy).

## When you also need a SQL migration

This backend targets an existing SQL Server database (DACPAC in `TKTDatabase/`). Schema changes belong in the DACPAC project, not in EF Core migrations. Do **not** run `dotnet ef migrations add` — the schema is managed outside EF.
