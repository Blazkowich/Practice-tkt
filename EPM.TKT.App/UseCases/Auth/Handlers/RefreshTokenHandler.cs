using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.App.UseCases.Auth.Commands;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Handlers;

internal sealed class RefreshTokenHandler(
    IUserRepository users,
    ITokenService tokens) : IRequestHandler<RefreshTokenCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var stored = await users.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (stored is null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            return Result<AuthResult>.Fail("Refresh token is invalid or expired.", ErrorType.Unauthorized);

        var user = await users.GetByIdAsync(stored.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return Result<AuthResult>.Fail("User not found or inactive.", ErrorType.Unauthorized);

        await users.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var accessToken     = tokens.GenerateAccessToken(user.UserId, user.Email, user.Roles);
        var newRefreshToken = tokens.GenerateRefreshToken();
        await users.SaveRefreshTokenAsync(user.UserId, newRefreshToken, tokens.GetRefreshTokenExpiry(), cancellationToken);

        return Result<AuthResult>.Ok(new AuthResult
        {
            AccessToken       = accessToken,
            RefreshToken      = newRefreshToken,
            AccessTokenExpiry = tokens.GetAccessTokenExpiry()
        });
    }
}
