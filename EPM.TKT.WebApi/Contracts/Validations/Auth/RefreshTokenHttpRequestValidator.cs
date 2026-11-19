using EPM.TKT.WebApi.Contracts.Requests.Auth;
using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Validations.Auth;

public sealed class RefreshTokenHttpRequestValidator : AbstractValidator<RefreshTokenHttpRequest>
{
    public RefreshTokenHttpRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
