using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Cart;

public sealed record AddToCartCommand(Guid CartId, int EventId, int VenueSeatId, int PriceId)
    : IRequest<Result<CartModel>>;

internal sealed class AddToCartHandler(ICartRepository carts)
    : IRequestHandler<AddToCartCommand, Result<CartModel>>
{
    public async Task<Result<CartModel>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await carts.AddSeatAsync(
            request.CartId, request.EventId, request.VenueSeatId, request.PriceId, cancellationToken);
        return Result<CartModel>.Ok(cart);
    }
}
