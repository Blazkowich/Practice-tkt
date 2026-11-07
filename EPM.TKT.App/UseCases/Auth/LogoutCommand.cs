using EPM.TKT.App.Repositories;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth;

public sealed record LogoutCommand(string RefreshToken) : IRequest;

internal sealed class LogoutHandler(IUserRepository users) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        => await users.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
}
