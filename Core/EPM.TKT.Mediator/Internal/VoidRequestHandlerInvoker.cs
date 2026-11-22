using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.Mediator.Internal;

internal sealed class VoidRequestHandlerInvoker<TRequest>(Type handlerImplementationType) : IRequestHandlerInvoker
    where TRequest : IRequest
{
    private readonly Type _handlerImplementationType = handlerImplementationType;

    public async Task<object?> InvokeAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var handler = (IRequestHandler<TRequest>)serviceProvider.GetRequiredService(_handlerImplementationType);
        await handler.HandleAsync((TRequest)request, cancellationToken).ConfigureAwait(false);
        return null;
    }
}
