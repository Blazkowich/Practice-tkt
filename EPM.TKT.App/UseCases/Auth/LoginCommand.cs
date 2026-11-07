using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResult>>;

internal sealed class LoginHandler(
    IUserRepository users,
    IPasswordService passwords,
    ITokenService tokens) : IRequestHandler<LoginCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive || user.PasswordHash is null
            || !passwords.Verify(request.Password, user.PasswordHash))
            return Result<AuthResult>.Fail("Invalid credentials.", ErrorType.Unauthorized);

        var accessToken  = tokens.GenerateAccessToken(user.UserId, user.Email, user.Roles);
        var refreshToken = tokens.GenerateRefreshToken();
        await users.SaveRefreshTokenAsync(user.UserId, refreshToken, tokens.GetRefreshTokenExpiry(), cancellationToken);

        return Result<AuthResult>.Ok(new AuthResult
        {
            AccessToken       = accessToken,
            RefreshToken      = refreshToken,
            AccessTokenExpiry = tokens.GetAccessTokenExpiry()
        });
    }
}
