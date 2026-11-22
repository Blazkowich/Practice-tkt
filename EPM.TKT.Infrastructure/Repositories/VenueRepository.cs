using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
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

    public async Task<VenueModel?> GetByIdAsync(
        int venueId, 
        CancellationToken ct = default
    ) => await context.Venues
            .AsNoTracking()
            .Where(v => v.VenueId == venueId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(
        string name, 
        string? address, 
        string? city, 
        string? country, 
        CancellationToken ct = default
    )
    {
        var venue = new Venue
        {
            Name = name,
            Address = address,
            City = city,
            Country = country,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        context.Venues.Add(venue);
        await context.SaveChangesAsync(ct);
        return venue.VenueId;
    }

    public async Task UpdateAsync(
        int venueId, 
        string name, 
        string? address, 
        string? city, 
        string? country, 
        CancellationToken ct = default
    ) => await context.Venues
            .Where(v => v.VenueId == venueId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Name, name)
                .SetProperty(x => x.Address, address)
                .SetProperty(x => x.City, city)
                .SetProperty(x => x.Country, country), ct);

    public async Task DeactivateAsync(
        int venueId, 
        CancellationToken ct = default
    ) => await context.Venues
            .Where(v => v.VenueId == venueId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false), ct);

    public async Task<IReadOnlyList<SectionModel>> GetSectionsAsync(
        int venueId, 
        CancellationToken ct = default
    ) => await context.Sections
            .AsNoTracking()
            .Where(s => s.VenueId == venueId)
            .OrderBy(s => s.SortOrder)
            .MapToDomainModel()
            .ToListAsync(ct);
}
