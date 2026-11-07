using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth;

public sealed record ResetPasswordCommand(string ResetToken, string NewPassword) : IRequest<Result>;

internal sealed class ResetPasswordHandler(
    IUserRepository users,
    IPasswordService passwords,
    ITokenService tokens) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = tokens.ValidatePasswordResetToken(request.ResetToken);
        if (userId is null)
            return Result.Fail("Reset token is invalid or expired.", ErrorType.Unauthorized);

        var user = await users.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
            return Result.Fail("User not found.", ErrorType.NotFound);

        var hash = passwords.Hash(request.NewPassword);
        await users.UpdatePasswordAsync(userId.Value, hash, cancellationToken);
        await users.RevokeAllRefreshTokensAsync(userId.Value, cancellationToken);

        return Result.Ok();
    }
}
