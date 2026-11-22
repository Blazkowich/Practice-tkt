using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Cart;

public sealed record RemoveFromCartCommand(Guid CartId, int EventId, int VenueSeatId) : IRequest<Result>;

internal sealed class RemoveFromCartHandler(ICartRepository carts)
    : IRequestHandler<RemoveFromCartCommand, Result>
{
    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        await carts.RemoveSeatAsync(request.CartId, request.EventId, request.VenueSeatId, cancellationToken);
        return Result.Ok();
    }
}
