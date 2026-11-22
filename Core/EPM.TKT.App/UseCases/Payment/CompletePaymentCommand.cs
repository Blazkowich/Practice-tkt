using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Payment;

public sealed record CompletePaymentCommand(Guid PaymentId) : IRequest<Result>;

internal sealed class CompletePaymentHandler(IPaymentRepository payments)
    : IRequestHandler<CompletePaymentCommand, Result>
{
    public async Task<Result> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await payments.GetAsync(request.PaymentId, cancellationToken);
        if (payment is null)
            return Result.Fail("Payment not found.", ErrorType.NotFound);

        await payments.CompleteAsync(request.PaymentId, cancellationToken);
        return Result.Ok();
    }
}
