using FluentValidation;

namespace EPM.TKT.WebApi.Contracts.Orders;

public sealed class AddToCartHttpRequestValidator : AbstractValidator<AddToCartHttpRequest>
{
    public AddToCartHttpRequestValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("EventId must be a positive integer.");

        RuleFor(x => x.SeatId)
            .GreaterThan(0).WithMessage("SeatId must be a positive integer.");

        RuleFor(x => x.PriceId)
            .GreaterThan(0).WithMessage("PriceId must be a positive integer.");
    }
}
