using EPM.TKT.Mediator.Exceptions;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.Mediator.Internal;

internal sealed class RequestHandlerProvider : IRequestHandlerProvider
{
    private readonly IReadOnlyDictionary<Type, IRequestHandlerInvoker> _byRequestType;

    public RequestHandlerProvider(IReadOnlyDictionary<Type, IRequestHandlerInvoker> byRequestType)
    {
        _byRequestType = byRequestType;
    }

    public Task<object?> InvokeAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestType = request.GetType();
        if (!_byRequestType.TryGetValue(requestType, out var invoker))
        {
            throw new HandlerNotRegisteredException(requestType);
        }

        return invoker.InvokeAsync(request, serviceProvider, cancellationToken);
    }
}
