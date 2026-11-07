using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.App.UseCases.Auth.Commands;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Auth;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth.Handlers;

internal sealed class RegisterHandler(
    IUserRepository users,
    IPasswordService passwords,
    ITokenService tokens) : IRequestHandler<RegisterCommand, Result<AuthResult>>
{
    public async Task<Result<AuthResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await users.EmailExistsAsync(request.Email, cancellationToken))
            return Result<AuthResult>.Fail("Email is already registered.", ErrorType.Conflict);

        var hash   = passwords.Hash(request.Password);
        var userId = await users.CreateUserAsync(request.Email, hash, request.FirstName, request.LastName, cancellationToken);

        var userInfo     = await users.GetByIdAsync(userId, cancellationToken);
        var accessToken  = tokens.GenerateAccessToken(userId, request.Email, userInfo!.Roles);
        var refreshToken = tokens.GenerateRefreshToken();
        await users.SaveRefreshTokenAsync(userId, refreshToken, tokens.GetRefreshTokenExpiry(), cancellationToken);

        return Result<AuthResult>.Ok(new AuthResult
        {
            AccessToken       = accessToken,
            RefreshToken      = refreshToken,
            AccessTokenExpiry = tokens.GetAccessTokenExpiry()
        });
    }
}
