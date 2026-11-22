using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class OfferRepository(ITKTDbContext context) : IOfferRepository
{
    public async Task<IReadOnlyList<OfferModel>> GetAllAsync(
        int eventId, 
        CancellationToken ct = default
    ) => await context.Offers
            .AsNoTracking()
            .Where(o => o.EventId == eventId)
            .OrderBy(o => o.SaleStartAt)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<OfferModel?> GetByIdAsync(
        int offerId, 
        CancellationToken ct = default
    ) => await context.Offers
            .AsNoTracking()
            .Where(o => o.OfferId == offerId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(
        int eventId, 
        string name, 
        DateTime saleStartAt, 
        DateTime? saleEndAt, 
        CancellationToken ct = default
    )
    {
        var offer = new Offer
        {
            EventId = eventId,
            Name = name,
            SaleStartAt = saleStartAt,
            SaleEndAt = saleEndAt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Offers.Add(offer);
        await context.SaveChangesAsync(ct);
        return offer.OfferId;
    }

    public async Task UpdateAsync(
        int offerId, 
        string name, 
        DateTime saleStartAt, 
        DateTime? saleEndAt, 
        bool isActive, 
        CancellationToken ct = default
    )
        => await context.Offers
            .Where(o => o.OfferId == offerId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Name, name)
                .SetProperty(x => x.SaleStartAt, saleStartAt)
                .SetProperty(x => x.SaleEndAt, saleEndAt)
                .SetProperty(x => x.IsActive, isActive), ct);

    public async Task DeleteAsync(
        int offerId, 
        CancellationToken ct = default
    ) => await context.Offers
            .Where(o => o.OfferId == offerId)
            .ExecuteDeleteAsync(ct);
}
