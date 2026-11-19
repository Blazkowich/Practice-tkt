using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed class RefreshTokenHttpRequestValidator : AbstractValidator<RefreshTokenHttpRequest>
{
    public RefreshTokenHttpRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
