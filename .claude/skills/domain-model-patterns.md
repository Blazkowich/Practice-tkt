The `EPM.TKT.Domain` project (namespace prefix: `EPM.TKT.Domain`) is dependency-free. Add domain models, enums, and exceptions here — never in App or Infrastructure.

## Models — `EPM.TKT.Domain/Models/[Domain]/[Name]Model.cs`

Use records for immutable read models and classes for models that need mutation.

```csharp
namespace EPM.TKT.Domain.Models.[Domain];

public sealed record [Name]Model
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public IReadOnlyList<ChildModel> Items { get; init; } = [];
}
```

Rules:
- One model per file; file name matches class name.
- Use `init`-only properties on records.
- Collections: `IReadOnlyList<T>`, never `List<T>` or arrays in the public API.
- Default non-nullable strings to `string.Empty`, not null.
- Nested / detail models go in the same file as inner classes:

```csharp
public sealed record [Name]Model
{
    public int Id { get; init; }
    public IReadOnlyList<ItemRow> Rows { get; init; } = [];

    public sealed record ItemRow
    {
        public int RowId { get; init; }
        public string Label { get; init; } = string.Empty;
    }
}
```

## Enums — `EPM.TKT.Domain/Enums/[Name].cs`

```csharp
namespace EPM.TKT.Domain.Enums;

public enum [Name] : short
{
    Option1 = 1,
    Option2 = 2,
}
```

- Explicit underlying type (`short` is the convention here).
- Explicit integer values — never rely on implicit zero-based ordering.
- One enum per file.

## Exceptions — `EPM.TKT.Domain/Exceptions/`

```csharp
namespace EPM.TKT.Domain.Exceptions;

public sealed class [Name]Exception : Exception
{
    public [Name]Exception(string message) : base(message) { }
    public [Name]Exception(string message, Exception inner) : base(message, inner) { }
}
```

## Namespace reminder

The project folder is `EPM.TKT.Domain` but the namespace root is `EPM.TKT.Domain` (no 's'). Always write `namespace EPM.TKT.Domain.*`.
