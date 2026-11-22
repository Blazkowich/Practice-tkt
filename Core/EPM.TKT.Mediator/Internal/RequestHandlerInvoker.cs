using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.Mediator.Internal;

internal sealed class RequestHandlerInvoker<TRequest, TResponse>(Type handlerImplementationType) : IRequestHandlerInvoker
    where TRequest : IRequest<TResponse>
{
    private readonly Type _handlerImplementationType = handlerImplementationType;

    public async Task<object?> InvokeAsync(
        object request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        var handler = (IRequestHandler<TRequest, TResponse>)serviceProvider.GetRequiredService(_handlerImplementationType);
        return await handler.HandleAsync((TRequest)request, cancellationToken).ConfigureAwait(false);
    }
}
