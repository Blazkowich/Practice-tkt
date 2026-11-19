using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class SectionRepository(ITKTDbContext context) : ISectionRepository
{
    public async Task<IReadOnlyList<SectionModel>> GetAllAsync(int venueId, CancellationToken ct = default)
        => await context.Sections
            .AsNoTracking()
            .Where(s => s.VenueId == venueId)
            .OrderBy(s => s.SortOrder)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<SectionModel?> GetByIdAsync(int sectionId, CancellationToken ct = default)
        => await context.Sections
            .AsNoTracking()
            .Where(s => s.SectionId == sectionId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(int venueId, string name, byte seatTypeId, int sortOrder, CancellationToken ct = default)
    {
        var section = new Section
        {
            VenueId   = venueId,
            Name      = name,
            SeatTypeId = seatTypeId,
            SortOrder = sortOrder
        };
        context.Sections.Add(section);
        await context.SaveChangesAsync(ct);
        return section.SectionId;
    }

    public async Task UpdateAsync(int sectionId, string name, byte seatTypeId, int sortOrder, CancellationToken ct = default)
        => await context.Sections
            .Where(s => s.SectionId == sectionId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Name, name)
                .SetProperty(x => x.SeatTypeId, seatTypeId)
                .SetProperty(x => x.SortOrder, sortOrder), ct);

    public async Task DeleteAsync(int sectionId, CancellationToken ct = default)
        => await context.Sections
            .Where(s => s.SectionId == sectionId)
            .ExecuteDeleteAsync(ct);
}
