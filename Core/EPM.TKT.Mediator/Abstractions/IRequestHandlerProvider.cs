namespace EPM.TKT.Mediator.Abstractions;

/// <summary>
/// Resolves and invokes the handler for a request instance (by runtime type).
/// </summary>
public interface IRequestHandlerProvider
{
    Task<object?> InvokeAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
