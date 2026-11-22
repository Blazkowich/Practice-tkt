using EPM.TKT.WebApi.Contracts.Requests.Auth;
using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Validations.Auth;

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
