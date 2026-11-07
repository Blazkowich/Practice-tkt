using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Payment;

public sealed record FailPaymentCommand(Guid PaymentId) : IRequest<Result>;

internal sealed class FailPaymentHandler(IPaymentRepository payments)
    : IRequestHandler<FailPaymentCommand, Result>
{
    public async Task<Result> Handle(FailPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await payments.GetAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Fail("Payment not found.", ErrorType.NotFound);

        await payments.FailAsync(request.PaymentId, cancellationToken);
        return Result.Ok();
    }
}
