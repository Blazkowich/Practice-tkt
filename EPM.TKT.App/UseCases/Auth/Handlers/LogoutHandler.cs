using EPM.TKT.App.Repositories;
using EPM.TKT.App.UseCases.Auth.Commands;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Handlers;

internal sealed class LogoutHandler(IUserRepository users) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        => await users.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
}
