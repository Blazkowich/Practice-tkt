using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.Mediator.Services;

/// <summary>
/// Default <see cref="ISender"/> implementation. Resolve as scoped (or transient) so handler dependencies match host lifetime.
/// </summary>
public sealed class Sender(IServiceProvider serviceProvider, IRequestHandlerProvider handlerProvider) : ISender
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IRequestHandlerProvider _handlerProvider = handlerProvider;

    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        var result = await _handlerProvider
            .InvokeAsync(request, _serviceProvider, ct)
            .ConfigureAwait(false);
        return (TResponse)result!;
    }

    public async Task SendAsync(
        IRequest request,
        CancellationToken ct = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        await _handlerProvider
            .InvokeAsync(request, _serviceProvider, ct)
            .ConfigureAwait(false);
    }
}
