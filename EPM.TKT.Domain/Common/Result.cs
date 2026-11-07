namespace EPM.TKT.Domain.Common;

public enum ErrorType
{
    Validation,   // → 400 Bad Request
    Unauthorized, // → 401 Unauthorized
    NotFound,     // → 404 Not Found
    Conflict,     // → 409 Conflict
}

public sealed record Error(string Message, ErrorType Type = ErrorType.Validation);

/// <summary>Use for operations that return a value or a business-rule failure.</summary>
public sealed class Result<T>
{
    private readonly T?     _value;
    private readonly Error? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public T     Value => IsSuccess  ? _value! : throw new InvalidOperationException("Cannot read Value of a failed Result.");
    public Error Error => IsFailure  ? _error! : throw new InvalidOperationException("Cannot read Error of a successful Result.");

    private Result(T value)    { IsSuccess = true;  _value = value; }
    private Result(Error error){ IsSuccess = false; _error = error; }

    public static Result<T> Ok(T value)   => new(value);
    public static Result<T> Fail(Error e) => new(e);
    public static Result<T> Fail(string message, ErrorType type = ErrorType.Validation)
        => new(new Error(message, type));

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure)
        => IsSuccess ? onSuccess(_value!) : onFailure(_error!);
}

/// <summary>Use for operations that produce no value but can fail.</summary>
public sealed class Result
{
    private readonly Error? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public Error Error => IsFailure ? _error! : throw new InvalidOperationException("Cannot read Error of a successful Result.");

    private Result()             { IsSuccess = true; }
    private Result(Error error)  { IsSuccess = false; _error = error; }

    public static Result Ok()         => new();
    public static Result Fail(Error e)=> new(e);
    public static Result Fail(string message, ErrorType type = ErrorType.Validation)
        => new(new Error(message, type));

    public TOut Match<TOut>(Func<TOut> onSuccess, Func<Error, TOut> onFailure)
        => IsSuccess ? onSuccess() : onFailure(_error!);
}
