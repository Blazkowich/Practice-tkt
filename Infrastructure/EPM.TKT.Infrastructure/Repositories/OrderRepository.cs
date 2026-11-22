using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Order;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class OrderRepository(ITKTDbContext context) : IOrderRepository
{
    public async Task<OrderModel?> GetByIdAsync(
        Guid orderId,
        CancellationToken ct = default
    ) => await context.Orders
            .AsNoTracking()
            .Where(o => o.OrderId == orderId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<OrderModel>> GetByUserAsync(
        Guid userId,
        CancellationToken ct = default
    ) => await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<Guid> CreateAsync(
        Guid userId,
        int eventId,
        Guid cartId,
        decimal totalAmount,
        CancellationToken ct = default
    )
    {
        var order = new Order
        {
            UserId = userId,
            EventId = eventId,
            CartId = cartId,
            TotalAmount = totalAmount,
            CreatedAt = DateTime.UtcNow
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);
        return order.OrderId;
    }

    public async Task UpdatePaymentTransactionAsync(
        Guid orderId,
        string paymentTransactionId,
        CancellationToken ct = default
    ) => await context.Orders
            .Where(o => o.OrderId == orderId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.PaymentTransactionId, paymentTransactionId), ct);
}
