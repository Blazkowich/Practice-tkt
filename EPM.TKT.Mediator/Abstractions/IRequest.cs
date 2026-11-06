namespace EPM.TKT.Mediator.Abstractions;

/// <summary>
/// Marker for a request that does not return a value. Use with <see cref="IRequestHandler{TRequest}"/> and <see cref="ISender.Send"/>.
/// </summary>
public interface IRequest { }

/// <summary>
/// Marker for a request that returns <typeparamref name="TResponse"/>.
/// </summary>
public interface IRequest<out TResponse> { }