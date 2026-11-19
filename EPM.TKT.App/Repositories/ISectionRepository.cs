using EPM.TKT.Domain.Models.Venue;

namespace EPM.TKT.App.Repositories;

public interface ISectionRepository
{
    Task<IReadOnlyList<SectionModel>> GetAllAsync(int venueId, CancellationToken ct = default);
    Task<SectionModel?> GetByIdAsync(int sectionId, CancellationToken ct = default);
    Task<int> CreateAsync(int venueId, string name, byte seatTypeId, int sortOrder, CancellationToken ct = default);
    Task UpdateAsync(int sectionId, string name, byte seatTypeId, int sortOrder, CancellationToken ct = default);
    Task DeleteAsync(int sectionId, CancellationToken ct = default);
}
