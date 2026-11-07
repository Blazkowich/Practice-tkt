using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Payment;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class PaymentRepository(ITKTDbContext context) : IPaymentRepository
{
    public async Task<PaymentModel?> GetAsync(Guid paymentId, CancellationToken ct = default)
    {
        var paymentIdStr = paymentId.ToString();

        var order = await context.Orders
            .AsNoTracking()
            .Where(o => o.PaymentTransactionId == paymentIdStr)
            .Select(o => new { o.OrderId, o.CartId, o.TotalAmount })
            .FirstOrDefaultAsync(ct);

        if (order is null) return null;

        var eventSeatIds = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == order.CartId)
            .Select(ci => ci.EventSeatId)
            .ToListAsync(ct);

        var states = await context.EventSeats
            .AsNoTracking()
            .Where(es => eventSeatIds.Contains(es.EventSeatId))
            .Select(es => es.SeatStateId)
            .ToListAsync(ct);

        var status = states.Count == 0 ? "Pending"
            : states.Any(s => s == 3)  ? "Completed"   // any Sold
            : states.All(s => s == 1)  ? "Failed"       // all Available
            : "Pending";

        return new PaymentModel
        {
            PaymentId   = paymentId,
            CartId      = order.CartId,
            Status      = status,
            TotalAmount = order.TotalAmount
        };
    }

    public async Task CompleteAsync(Guid paymentId, CancellationToken ct = default)
    {
        var paymentIdStr = paymentId.ToString();

        var cartId = await context.Orders
            .AsNoTracking()
            .Where(o => o.PaymentTransactionId == paymentIdStr)
            .Select(o => o.CartId)
            .FirstOrDefaultAsync(ct);

        var eventSeatIds = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .Select(ci => ci.EventSeatId)
            .ToListAsync(ct);

        await context.EventSeats
            .Where(es => eventSeatIds.Contains(es.EventSeatId) && es.SeatStateId == 2)
            .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)3), ct);
    }

    public async Task FailAsync(Guid paymentId, CancellationToken ct = default)
    {
        var paymentIdStr = paymentId.ToString();

        var cartId = await context.Orders
            .AsNoTracking()
            .Where(o => o.PaymentTransactionId == paymentIdStr)
            .Select(o => o.CartId)
            .FirstOrDefaultAsync(ct);

        var eventSeatIds = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .Select(ci => ci.EventSeatId)
            .ToListAsync(ct);

        await context.EventSeats
            .Where(es => eventSeatIds.Contains(es.EventSeatId) && es.SeatStateId == 2)
            .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)1), ct);
    }
}
