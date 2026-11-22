using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Payment;
using Microsoft.EntityFrameworkCore;
using OrderStatusType = EPM.TKT.Domain.Enums.OrderStatus;
using SeatStateType = EPM.TKT.Domain.Enums.SeatState;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class PaymentRepository(ITKTDbContext context) : IPaymentRepository
{
    public async Task<PaymentModel?> GetAsync(
        Guid paymentId,
        CancellationToken ct = default
    )
    {
        var paymentIdStr = paymentId.ToString();

        var order = await context.Orders
            .AsNoTracking()
            .Where(o => o.PaymentTransactionId == paymentIdStr)
            .Select(o => new { o.CartId, o.TotalAmount, o.OrderStatusId })
            .FirstOrDefaultAsync(ct);

        if (order is null) return null;

        return new PaymentModel
        {
            PaymentId   = paymentId,
            CartId      = order.CartId,
            Status      = ((OrderStatusType)order.OrderStatusId).ToString(),
            TotalAmount = order.TotalAmount
        };
    }

    public async Task CompleteAsync(
        Guid paymentId, 
        CancellationToken ct = default
    )
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

        await context.ExecuteInTransactionAsync<bool>(async token =>
        {
            await context.EventSeats
                .Where(es => eventSeatIds.Contains(es.EventSeatId)
                          && es.SeatStateId == (byte)SeatStateType.Booked)
                .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)SeatStateType.Sold), token);

            await context.Orders
                .Where(o => o.PaymentTransactionId == paymentIdStr)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.OrderStatusId, (byte)OrderStatusType.Paid), token);

            return true;
        }, ct);
    }

    public async Task FailAsync(
        Guid paymentId, 
        CancellationToken ct = default
    )
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

        await context.ExecuteInTransactionAsync<bool>(async token =>
        {
            await context.EventSeats
                .Where(es => eventSeatIds.Contains(es.EventSeatId)
                          && es.SeatStateId == (byte)SeatStateType.Booked)
                .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)SeatStateType.Available), token);

            await context.Orders
                .Where(o => o.PaymentTransactionId == paymentIdStr)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.OrderStatusId, (byte)OrderStatusType.Cancelled), token);

            return true;
        }, ct);
    }
}
