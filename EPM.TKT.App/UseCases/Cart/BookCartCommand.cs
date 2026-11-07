using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Cart;

public sealed record BookCartCommand(Guid CartId) : IRequest<Result<Guid>>;

internal sealed class BookCartHandler(ICartRepository carts)
    : IRequestHandler<BookCartCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(BookCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await carts.GetAsync(request.CartId, cancellationToken);
        if (cart is null)
            return Result<Guid>.Fail("Cart not found.", ErrorType.NotFound);
        if (cart.Items.Count == 0)
            return Result<Guid>.Fail("Cart is empty.", ErrorType.Validation);

        var paymentId = await carts.BookAsync(request.CartId, cancellationToken);
        return Result<Guid>.Ok(paymentId);
    }
}
