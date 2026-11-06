using EPM.TKT.Mediator.Exceptions;

namespace EPM.TKT.Mediator.Abstractions;

/// <summary>
/// Dispatches requests to their registered handlers.
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends a request through the pipeline and returns the handler result.
    /// </summary>
    /// <exception cref="HandlerNotRegisteredException">No handler registered for the request runtime type.</exception>
    Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request with no return value through the pipeline.
    /// </summary>
    /// <exception cref="HandlerNotRegisteredException">No handler registered for the request runtime type.</exception>
    Task Send(IRequest request, CancellationToken cancellationToken = default);
}
