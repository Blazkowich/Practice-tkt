using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class PriceRepository(ITKTDbContext context) : IPriceRepository
{
    public async Task<IReadOnlyList<PriceModel>> GetAllAsync(int offerId, CancellationToken ct = default)
        => await context.Prices
            .AsNoTracking()
            .Where(p => p.OfferId == offerId)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<PriceModel?> GetByIdAsync(int priceId, CancellationToken ct = default)
        => await context.Prices
            .AsNoTracking()
            .Where(p => p.PriceId == priceId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(int offerId, int? sectionId, int? rowId, byte priceCategoryId, decimal amount, string currency, CancellationToken ct = default)
    {
        var price = new Price
        {
            OfferId         = offerId,
            SectionId       = sectionId,
            RowId           = rowId,
            PriceCategoryId = priceCategoryId,
            Amount          = amount,
            Currency        = currency
        };
        context.Prices.Add(price);
        await context.SaveChangesAsync(ct);
        return price.PriceId;
    }

    public async Task UpdateAsync(int priceId, decimal amount, string currency, CancellationToken ct = default)
        => await context.Prices
            .Where(p => p.PriceId == priceId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Amount, amount)
                .SetProperty(x => x.Currency, currency), ct);

    public async Task DeleteAsync(int priceId, CancellationToken ct = default)
        => await context.Prices
            .Where(p => p.PriceId == priceId)
            .ExecuteDeleteAsync(ct);
}
