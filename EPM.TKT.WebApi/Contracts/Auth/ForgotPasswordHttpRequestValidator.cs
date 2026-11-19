using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed class ForgotPasswordHttpRequestValidator : AbstractValidator<ForgotPasswordHttpRequest>
{
    public ForgotPasswordHttpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.");
    }
}
