using EPM.TKT.Domain.Models.Cart;

namespace EPM.TKT.App.Repositories;

public interface ICartRepository
{
    Task<CartModel?> GetAsync(Guid cartId, CancellationToken ct = default);
    Task<CartModel> AddSeatAsync(Guid cartId, Guid userId, int eventId, int venueSeatId, int priceId, CancellationToken ct = default);
    Task RemoveSeatAsync(Guid cartId, int eventId, int venueSeatId, CancellationToken ct = default);
    Task<Guid> BookAsync(Guid cartId, Guid userId, CancellationToken ct = default);
}
