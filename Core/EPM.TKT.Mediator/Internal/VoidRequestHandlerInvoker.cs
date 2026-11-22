using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.Mediator.Internal;

internal sealed class VoidRequestHandlerInvoker<TRequest> : IRequestHandlerInvoker
    where TRequest : IRequest
{
    private readonly Type _handlerImplementationType;

    public VoidRequestHandlerInvoker(Type handlerImplementationType)
    {
        _handlerImplementationType = handlerImplementationType;
    }

    public async Task<object?> InvokeAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var handler = (IRequestHandler<TRequest>)serviceProvider.GetRequiredService(_handlerImplementationType);
        await handler.Handle((TRequest)request, cancellationToken).ConfigureAwait(false);
        return null;
    }
}
