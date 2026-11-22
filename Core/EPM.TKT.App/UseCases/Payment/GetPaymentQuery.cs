using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Payment;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Payment;

public sealed record GetPaymentQuery(Guid PaymentId) : IRequest<Result<PaymentModel>>;

internal sealed class GetPaymentHandler(IPaymentRepository payments)
    : IRequestHandler<GetPaymentQuery, Result<PaymentModel>>
{
    public async Task<Result<PaymentModel>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await payments.GetAsync(request.PaymentId, cancellationToken);
        return payment is null
            ? Result<PaymentModel>.Fail("Payment not found.", ErrorType.NotFound)
            : Result<PaymentModel>.Ok(payment);
    }
}
