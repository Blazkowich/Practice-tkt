using EPM.TKT.Domain.Models.Event;

namespace EPM.TKT.App.Repositories;

public interface IOfferRepository
{
    Task<IReadOnlyList<OfferModel>> GetAllAsync(
        int eventId, 
        CancellationToken ct = default
    );
    Task<OfferModel?> GetByIdAsync(
        int offerId, 
        CancellationToken ct = default
    );
    Task<int> CreateAsync(
        int eventId, 
        string name, 
        DateTime saleStartAt, 
        DateTime? saleEndAt, 
        CancellationToken ct = default
    );
    Task UpdateAsync(
        int offerId, 
        string name, 
        DateTime saleStartAt, 
        DateTime? saleEndAt, 
        bool isActive, 
        CancellationToken ct = default
    );
    Task DeleteAsync(
        int offerId, 
        CancellationToken ct = default
    );
}
