using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class CartRepository(
    ITKTDbContext context,
    IPaymentServiceClient paymentServiceClient) : ICartRepository
{
    public async Task<CartModel?> GetAsync(Guid cartId, CancellationToken ct = default)
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
                SeatNumber  = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                    ? ci.EventSeat.VenueSeat.SeatNumber : null,
                RowName     = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                              && ci.EventSeat.VenueSeat.Row != null
                    ? ci.EventSeat.VenueSeat.Row.Name : null,
                SectionName = ci.EventSeat != null && ci.EventSeat.VenueSeat != null
                              && ci.EventSeat.VenueSeat.Section != null
                    ? ci.EventSeat.VenueSeat.Section.Name : string.Empty,
                PriceId     = ci.EventSeat != null && ci.EventSeat.PriceId.HasValue
                    ? ci.EventSeat.PriceId.Value : 0,
                PriceName   = ci.EventSeat != null && ci.EventSeat.Price != null
                              && ci.EventSeat.Price.PriceCategory != null
                    ? ci.EventSeat.Price.PriceCategory.Name : string.Empty,
                Amount      = ci.EventSeat != null && ci.EventSeat.Price != null
                    ? ci.EventSeat.Price.Amount : 0,
                Currency    = ci.EventSeat != null && ci.EventSeat.Price != null
                    ? ci.EventSeat.Price.Currency : "USD"
            })
            .ToListAsync(ct);

        return new CartModel
        {
            CartId      = cart.CartId,
            EventId     = cart.EventId,
            Items       = items,
            TotalAmount = items.Sum(i => i.Amount)
        };
    }

    public async Task<CartModel> AddSeatAsync(
        Guid cartId, int eventId, int venueSeatId, int priceId, CancellationToken ct = default)
    {
        var cartExists = await context.Carts.AnyAsync(c => c.CartId == cartId, ct);
        if (!cartExists)
        {
            context.Carts.Add(new Cart
            {
                CartId    = cartId,
                UserId    = Guid.Empty,
                EventId   = eventId,
                CreatedAt = DateTime.UtcNow
            });
        }

        var eventSeat = await context.EventSeats
            .Where(es => es.EventId == eventId && es.VenueSeatId == venueSeatId)
            .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException($"EventSeat not found for event {eventId}, seat {venueSeatId}.");

        if (eventSeat.PriceId is null)
            eventSeat.PriceId = priceId;

        var alreadyInCart = await context.CartItems
            .AnyAsync(ci => ci.CartId == cartId && ci.EventSeatId == eventSeat.EventSeatId, ct);

        if (!alreadyInCart)
        {
            context.CartItems.Add(new CartItem
            {
                CartId      = cartId,
                EventSeatId = eventSeat.EventSeatId,
                AddedAt     = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync(ct);
        return (await GetAsync(cartId, ct))!;
    }

    public async Task RemoveSeatAsync(Guid cartId, int eventId, int venueSeatId, CancellationToken ct = default)
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

    public async Task<Guid> BookAsync(Guid cartId, CancellationToken ct = default)
    {
        var cart = await context.Carts
            .AsNoTracking()
            .Where(c => c.CartId == cartId)
            .Select(c => new { c.CartId, c.EventId })
            .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException("Cart not found.");

        var eventSeatIds = await context.CartItems
            .AsNoTracking()
            .Where(ci => ci.CartId == cartId)
            .Select(ci => ci.EventSeatId)
            .ToListAsync(ct);

        // Mark seats as Booked (state = 2)
        await context.EventSeats
            .Where(es => eventSeatIds.Contains(es.EventSeatId))
            .ExecuteUpdateAsync(s => s.SetProperty(es => es.SeatStateId, (byte)2), ct);

        // Compute total from locked prices
        var total = await context.EventSeats
            .AsNoTracking()
            .Where(es => eventSeatIds.Contains(es.EventSeatId) && es.Price != null)
            .SumAsync(es => es.Price!.Amount, ct);

        // Create Order record
        var order = new Order
        {
            CartId    = cartId,
            UserId    = Guid.Empty,
            EventId   = cart.EventId,
            TotalAmount = total,
            CreatedAt = DateTime.UtcNow
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        // Call external Payment Service to initiate payment
        var paymentId = await paymentServiceClient.InitiatePaymentAsync(order.OrderId, total, ct);

        // Store the external payment transaction ID on the Order
        await context.Orders
            .Where(o => o.OrderId == order.OrderId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.PaymentTransactionId, paymentId.ToString()), ct);

        return paymentId;
    }
}
