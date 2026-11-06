namespace EPM.TKT.Mediator.Exceptions;

/// <summary>
/// Thrown when <see cref="Abstractions.ISender"/> dispatch is invoked for a request type with no handler.
/// </summary>
public sealed class HandlerNotRegisteredException : InvalidOperationException
{
    public HandlerNotRegisteredException(Type requestType)
        : base($"No IRequestHandler is registered for request type '{requestType.FullName}'.")
    {
        RequestType = requestType;
    }

    public Type RequestType { get; }
}
