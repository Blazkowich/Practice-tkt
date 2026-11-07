Use `Result<T>` / `Result` (from `EPM.TKT.Domain.Common`) for every handler that has a business-rule failure path. Never throw `DomainException` from a handler — return a `Result.Fail(...)` instead.

## Types

```csharp
// value-producing operation
Result<T>.Ok(value)
Result<T>.Fail("message", ErrorType.Conflict)

// void operation
Result.Ok()
Result.Fail("message", ErrorType.Unauthorized)
```

## ErrorType → HTTP status

| `ErrorType` | HTTP status |
|---|---|
| `Validation` (default) | 400 Bad Request |
| `Unauthorized` | 401 Unauthorized |
| `NotFound` | 404 Not Found |
| `Conflict` | 409 Conflict |

## Handler pattern

```csharp
// Command signature
public sealed record CreateFooCommand(...) : IRequest<Result<FooModel>>;

// Handler
internal sealed class CreateFooHandler(...) : IRequestHandler<CreateFooCommand, Result<FooModel>>
{
    public async Task<Result<FooModel>> Handle(CreateFooCommand request, CancellationToken cancellationToken)
    {
        if (duplicateCondition)
            return Result<FooModel>.Fail("Foo already exists.", ErrorType.Conflict);

        // ... success path ...
        return Result<FooModel>.Ok(model);
    }
}
```

For void commands:
```csharp
public sealed record DeleteFooCommand(int Id) : IRequest<Result>;

// handler returns Result.Ok() or Result.Fail(...)
```

## Controller pattern

Use `Match` — no try/catch:

```csharp
var result = await sender.Send(command, ct);

return result.Match(
    onSuccess: value => Ok(Map(value)),
    onFailure: error => Problem(error));  // Problem() is the private helper below
```

Every controller must have this private helper to map `ErrorType` to HTTP status:

```csharp
private IActionResult Problem(Error error) => error.Type switch
{
    ErrorType.Unauthorized => Unauthorized(new { error.Message }),
    ErrorType.NotFound     => NotFound(new { error.Message }),
    ErrorType.Conflict     => Conflict(new { error.Message }),
    _                      => BadRequest(new { error.Message })
};
```

## When NOT to use Result

- `ForgotPassword` — always returns 200 OK by design (never reveals whether email exists)
- `Logout` — idempotent, no meaningful failure
- Infrastructure exceptions (DB down, network error) — let those propagate as unhandled exceptions to the global error handler
