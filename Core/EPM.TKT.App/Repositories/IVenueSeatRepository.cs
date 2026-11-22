using EPM.TKT.Domain.Models.Venue;

namespace EPM.TKT.App.Repositories;

public interface IVenueSeatRepository
{
    Task<IReadOnlyList<VenueSeatModel>> GetAllAsync(
        int sectionId, 
        CancellationToken ct = default
    );
    Task<VenueSeatModel?> GetByIdAsync(
        int venueSeatId, 
        CancellationToken ct = default
    );
    Task<int> CreateAsync(
        int sectionId, 
        int? rowId, 
        string? seatNumber, 
        int sortOrder, 
        CancellationToken ct = default
    );
    Task UpdateAsync(
        int venueSeatId, 
        string? seatNumber, 
        int sortOrder, 
        CancellationToken ct = default
    );
    Task DeleteAsync(
        int venueSeatId, 
        CancellationToken ct = default
    );
}
