using EPM.TKT.Domain.Models.Payment;

namespace EPM.TKT.App.Repositories;

public interface IPaymentRepository
{
    Task<PaymentModel?> GetAsync(Guid paymentId, CancellationToken ct = default);
    Task CompleteAsync(Guid paymentId, CancellationToken ct = default);
    Task FailAsync(Guid paymentId, CancellationToken ct = default);
}
