namespace EPM.TKT.Mediator.Abstractions;

/// <summary>
/// Handles a specific <typeparamref name="TRequest"/> with no return value.
/// </summary>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    Task Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Handles a specific <typeparamref name="TRequest"/> and produces <typeparamref name="TResponse"/>.
/// </summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
