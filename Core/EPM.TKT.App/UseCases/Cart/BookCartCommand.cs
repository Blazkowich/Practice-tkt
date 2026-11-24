using EPM.TKT.App.Cache;
using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace EPM.TKT.App.UseCases.Cart;

public sealed record BookCartCommand(Guid CartId, Guid UserId) : IRequest<Result<Guid>>;

internal sealed class BookCartHandler(ICartRepository carts, IMemoryCache cache)
    : IRequestHandler<BookCartCommand, Result<Guid>>
{
    public async Task<Result<Guid>> HandleAsync(BookCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await carts.GetAsync(request.CartId, cancellationToken);
        if (cart is null)
            return Result<Guid>.Fail("Cart not found.", ErrorType.NotFound);
        if (cart.Items.Count == 0)
            return Result<Guid>.Fail("Cart is empty.", ErrorType.BadRequest);

        var paymentId = await carts.BookAsync(request.CartId, request.UserId, cancellationToken);

        InvalidateSeatCache(cart.EventId);

        return Result<Guid>.Ok(paymentId);
    }

    private void InvalidateSeatCache(int eventId)
    {
        var genKey = EventCacheKeys.EventSeatGeneration(eventId);
        var generation = cache.GetOrCreate(genKey, _ => 0);
        cache.Set(genKey, generation + 1);
    }
}
