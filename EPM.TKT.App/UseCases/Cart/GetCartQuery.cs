using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Cart;

public sealed record GetCartQuery(Guid CartId) : IRequest<CartModel?>;

internal sealed class GetCartHandler(ICartRepository carts)
    : IRequestHandler<GetCartQuery, CartModel?>
{
    public Task<CartModel?> Handle(GetCartQuery request, CancellationToken cancellationToken)
        => carts.GetAsync(request.CartId, cancellationToken);
}
