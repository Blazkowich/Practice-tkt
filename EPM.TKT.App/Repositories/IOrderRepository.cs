using EPM.TKT.Domain.Models.Order;

namespace EPM.TKT.App.Repositories;

public interface IOrderRepository
{
    Task<OrderModel?> GetByIdAsync(Guid orderId, CancellationToken ct = default);
    Task<IReadOnlyList<OrderModel>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Guid> CreateAsync(Guid userId, int eventId, Guid cartId, decimal totalAmount, CancellationToken ct = default);
    Task UpdatePaymentTransactionAsync(Guid orderId, string paymentTransactionId, CancellationToken ct = default);
}
