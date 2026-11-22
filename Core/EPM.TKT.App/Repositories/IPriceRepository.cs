using EPM.TKT.Domain.Enums;
using EPM.TKT.Domain.Models.Event;

namespace EPM.TKT.App.Repositories;

public interface IPriceRepository
{
    Task<IReadOnlyList<PriceModel>> GetAllAsync(
        int offerId,
        CancellationToken ct = default
    );
    Task<PriceModel?> GetByIdAsync(
        int priceId,
        CancellationToken ct = default
    );
    Task<int> CreateAsync(
        int offerId,
        int? sectionId,
        int? rowId,
        PriceCategory priceCategory,
        decimal amount,
        string currency,
        CancellationToken ct = default
    );
    Task UpdateAsync(
        int priceId,
        decimal amount,
        string currency,
        CancellationToken ct = default
    );
    Task DeleteAsync(
        int priceId,
        CancellationToken ct = default
    );
}
