using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Auth;

public sealed class ResetPasswordHttpRequestValidator : AbstractValidator<ResetPasswordHttpRequest>
{
    public ResetPasswordHttpRequestValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty().WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
            .MaximumLength(128).WithMessage("New password must not exceed 128 characters.");
    }
}
