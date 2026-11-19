using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed class LoginHttpRequestValidator : AbstractValidator<LoginHttpRequest>
{
    public LoginHttpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
