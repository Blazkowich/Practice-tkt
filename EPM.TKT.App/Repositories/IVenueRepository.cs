using EPM.TKT.Domain.Models.Venue;

namespace EPM.TKT.App.Repositories;

public interface IVenueRepository
{
    Task<IReadOnlyList<VenueModel>> GetAllAsync(CancellationToken ct = default);
    Task<VenueModel?> GetByIdAsync(int venueId, CancellationToken ct = default);
    Task<int> CreateAsync(string name, string? address, string? city, string? country, CancellationToken ct = default);
    Task UpdateAsync(int venueId, string name, string? address, string? city, string? country, CancellationToken ct = default);
    Task DeactivateAsync(int venueId, CancellationToken ct = default);
    Task<IReadOnlyList<SectionModel>> GetSectionsAsync(int venueId, CancellationToken ct = default);
}
