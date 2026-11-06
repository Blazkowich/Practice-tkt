namespace EPM.TKT.Mediator.Internal;

internal interface IRequestHandlerInvoker
{
    Task<object?> InvokeAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}
