using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Auth;

// Returns the reset token (dev convenience). In production send via email instead.
public sealed record ForgotPasswordCommand(string Email) : IRequest<string?>;

internal sealed class ForgotPasswordHandler(
    IUserRepository users,
    ITokenService tokens) : IRequestHandler<ForgotPasswordCommand, string?>
{
    public async Task<string?> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !user.IsActive)
            return null; // always succeed — never reveal whether email exists

        return tokens.GeneratePasswordResetToken(user.UserId);
    }
}
