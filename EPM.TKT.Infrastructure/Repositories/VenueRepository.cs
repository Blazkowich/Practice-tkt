using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class VenueRepository(ITKTDbContext context) : IVenueRepository
{
    public async Task<IReadOnlyList<VenueModel>> GetAllAsync(CancellationToken ct = default)
        => await context.Venues
            .AsNoTracking()
            .Where(v => v.IsActive)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<SectionModel>> GetSectionsAsync(int venueId, CancellationToken ct = default)
        => await context.Sections
            .AsNoTracking()
            .Where(s => s.VenueId == venueId)
            .MapToDomainModel()
            .ToListAsync(ct);
}
