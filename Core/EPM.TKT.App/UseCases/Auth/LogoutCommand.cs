using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth;

public sealed record LogoutCommand(string RefreshToken) : IRequest;

internal sealed class LogoutHandler : IRequestHandler<LogoutCommand>
{
    public Task HandleAsync(LogoutCommand request, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
