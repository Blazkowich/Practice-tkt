using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.Mediator.Services;

/// <summary>
/// Default <see cref="ISender"/> implementation. Resolve as scoped (or transient) so handler dependencies match host lifetime.
/// </summary>
public sealed class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IRequestHandlerProvider _handlerProvider;

    public Sender(IServiceProvider serviceProvider, IRequestHandlerProvider handlerProvider)
    {
        _serviceProvider = serviceProvider;
        _handlerProvider = handlerProvider;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var result = await _handlerProvider
            .InvokeAsync(request, _serviceProvider, cancellationToken)
            .ConfigureAwait(false);
        return (TResponse)result!;
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await _handlerProvider
            .InvokeAsync(request, _serviceProvider, cancellationToken)
            .ConfigureAwait(false);
    }
}
