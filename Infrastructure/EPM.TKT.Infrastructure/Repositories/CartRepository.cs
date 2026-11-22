using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;
using SeatStateType = EPM.TKT.Domain.Enums.SeatState;
using OrderStatusType = EPM.TKT.Domain.Enums.OrderStatus;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class CartRepository(
    ITKTDbContext context,
    IPaymentServiceClient paymentServiceClient
) : ICartRepository
{
    public async Task<CartModel?> GetAsync(
        Guid cartId,
        CancellationToken ct = default
    )
    {
        var cart = await context.Carts
            .AsNoTracking()
            .Where(c => c.CartId == cartId)
            .Select(c => new { c.CartId, c.EventId })
            .FirstOrDefaultAsync(ct);

        if (cart is null) return null;

        var items = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .Select(ci => new CartItemModel
            {
                EventSeatId = ci.EventSeatId,
                VenueSeatId = ci.EventSeat != null ? ci.EventSeat.VenueSeatId : 0,
                SeatNumber = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                    ? ci.EventSeat.VenueSeat.SeatNumber : null,
                RowName = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                              && ci.EventSeat.VenueSeat.Row != null
                    ? ci.EventSeat.VenueSeat.Row.Name : null,
                SectionName = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                              && ci.EventSeat.VenueSeat.Section != null
                    ? ci.EventSeat.VenueSeat.Section.Name : string.Empty,
                PriceId = ci.EventSeat != null && ci.EventSeat.PriceId.HasValue
                    ? ci.EventSeat.PriceId.Value : 0,
                PriceName = ci.EventSeat != null && ci.EventSeat.Price != null
                              && ci.EventSeat.Price.PriceCategory != null
                    ? ci.EventSeat.Price.PriceCategory.Name : string.Empty,
                Amount = ci.EventSeat != null && ci.EventSeat.Price != null
                    ? ci.EventSeat.Price.Amount : 0,
                Currency = ci.EventSeat != null && ci.EventSeat.Price != null
                    ? ci.EventSeat.Price.Currency : "USD"
            })
            .ToListAsync(ct);

        return new CartModel
        {
            CartId = cart.CartId,
            EventId = cart.EventId,
            Items = items,
            TotalAmount = items.Sum(i => i.Amount)
        };
    }

    public async Task<CartModel> AddSeatAsync(
        Guid cartId,
        Guid userId,
        int eventId,
        int venueSeatId,
        int priceId,
        CancellationToken ct = default
    )
    {
        return await context.ExecuteInTransactionAsync<CartModel>(async token =>
        {
            var cartExists = await context.Carts
                .AnyAsync(c => c.CartId == cartId, token);

            if (!cartExists)
            {
                context.Carts.Add(new Cart
                {
                    CartId = cartId,
                    UserId = userId,
                    EventId = eventId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var eventSeat = await context.EventSeats
                .Where(es => es.EventId == eventId && es.VenueSeatId == venueSeatId)
                .FirstOrDefaultAsync(token)
                ?? throw new InvalidOperationException($"EventSeat not found for event {eventId}, seat {venueSeatId}.");

            eventSeat.PriceId ??= priceId;

            var alreadyInCart = await context.CartItems
                .AnyAsync(ci =>
                    ci.CartId == cartId &&
                    ci.EventSeatId == eventSeat.EventSeatId,
                token);

            if (!alreadyInCart)
            {
                context.CartItems.Add(new CartItem
                {
                    CartId = cartId,
                    EventSeatId = eventSeat.EventSeatId,
                    AddedAt = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync(token);
            return (await GetAsync(cartId, token))!;
        }, ct);
    }

    public async Task RemoveSeatAsync(
        Guid cartId,
        int eventId,
        int venueSeatId,
        CancellationToken ct = default
    )
    {
        var eventSeatId = await context.EventSeats
            .AsNoTracking()
            .Where(es => es.EventId == eventId && es.VenueSeatId == venueSeatId)
            .Select(es => es.EventSeatId)
            .FirstOrDefaultAsync(ct);

        if (eventSeatId == 0) return;

        await context.CartItems
            .Where(ci => ci.CartId == cartId && ci.EventSeatId == eventSeatId)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<Guid> BookAsync(
        Guid cartId,
        Guid userId,
        CancellationToken ct = default
    )
    {
        var (orderId, total) = await context.ExecuteInTransactionAsync<(Guid orderId, decimal total)>(async token =>
        {
            var cart = await context.Carts
                .AsNoTracking()
                .Where(c => c.CartId == cartId)
                .Select(c => new { c.CartId, c.EventId })
                .FirstOrDefaultAsync(token)
                ?? throw new InvalidOperationException("Cart not found.");

            var eventSeatIds = await context.CartItems
                .AsNoTracking()
                .Where(ci => ci.CartId == cartId)
                .Select(ci => ci.EventSeatId)
                .ToListAsync(token);

            if (eventSeatIds.Count == 0)
                throw new InvalidOperationException("Cart is empty.");

            var updatedSeats = await context.EventSeats
                .Where(es => eventSeatIds.Contains(es.EventSeatId)
                          && es.SeatStateId == (byte)SeatStateType.Available)
                .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)SeatStateType.Booked), token);

            if (updatedSeats != eventSeatIds.Count)
                throw new InvalidOperationException("Some seats are no longer available.");

            var amounts = await context.EventSeats
                .AsNoTracking()
                .Where(es => eventSeatIds.Contains(es.EventSeatId))
                .Select(es => es.Price != null ? (decimal?)es.Price.Amount : null)
                .ToListAsync(token);

            if (amounts.Any(a => a is null))
                throw new InvalidOperationException("Some seats have no price.");

            var totalAmount = amounts.Sum(a => a!.Value);

            var order = new Order
            {
                CartId = cartId,
                UserId = userId,
                EventId = cart.EventId,
                TotalAmount = totalAmount,
                OrderStatusId = (byte)OrderStatusType.Pending,
                CreatedAt = DateTime.UtcNow
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync(token);

            return (order.OrderId, totalAmount);
        }, ct);

        Guid paymentId;
        try
        {
            paymentId = await paymentServiceClient.InitiatePaymentAsync(orderId, total, ct);
        }
        catch
        {
            await CancelOrderAsync(orderId, cartId, CancellationToken.None);
            throw;
        }

        await context.Orders
            .Where(o => o.OrderId == orderId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.PaymentTransactionId, paymentId.ToString()), ct);

        return paymentId;
    }

    private Task CancelOrderAsync(Guid orderId, Guid cartId, CancellationToken ct) =>
        context.ExecuteInTransactionAsync(async token =>
        {
            var cancelled = await context.Orders
                .Where(o => o.OrderId == orderId
                         && o.OrderStatusId == (byte)OrderStatusType.Pending)
                .ExecuteUpdateAsync(s => s.SetProperty(o => o.OrderStatusId, (byte)OrderStatusType.Cancelled), token);

            if (cancelled == 0) return;

            var seatIds = await context.CartItems
                .AsNoTracking()
                .Where(ci => ci.CartId == cartId)
                .Select(ci => ci.EventSeatId)
                .ToListAsync(token);

            await context.EventSeats
                .Where(es => seatIds.Contains(es.EventSeatId)
                          && es.SeatStateId == (byte)SeatStateType.Booked)
                .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)SeatStateType.Available), token);
        }, ct);
}