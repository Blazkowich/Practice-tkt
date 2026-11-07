using EPM.TKT.Domain.Models.Venue;

namespace EPM.TKT.App.Repositories;

public interface IVenueRepository
{
    Task<IReadOnlyList<VenueModel>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SectionModel>> GetSectionsAsync(int venueId, CancellationToken ct = default);
}
