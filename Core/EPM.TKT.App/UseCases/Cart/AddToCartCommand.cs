using EPM.TKT.App.Cache;
using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace EPM.TKT.App.UseCases.Cart;

public sealed record AddToCartCommand(Guid CartId, Guid UserId, int EventId, int VenueSeatId, int PriceId)
    : IRequest<Result<CartModel>>;

internal sealed class AddToCartHandler(ICartRepository carts, IMemoryCache cache)
    : IRequestHandler<AddToCartCommand, Result<CartModel>>
{
    public async Task<Result<CartModel>> HandleAsync(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await carts.AddSeatAsync(
            request.CartId, request.UserId, request.EventId, request.VenueSeatId, request.PriceId, cancellationToken);

        InvalidateSeatCache(request.EventId);

        return Result<CartModel>.Ok(cart);
    }

    private void InvalidateSeatCache(int eventId)
    {
        var genKey = EventCacheKeys.EventSeatGeneration(eventId);
        var generation = cache.GetOrCreate(genKey, _ => 0);
        cache.Set(genKey, generation + 1);
    }
}
