using EPM.TKT.Domain.Models.Venue;

namespace EPM.TKT.App.Repositories;

public interface IRowRepository
{
    Task<IReadOnlyList<RowModel>> GetAllAsync(
        int sectionId, 
        CancellationToken ct = default
    );
    Task<RowModel?> GetByIdAsync(
        int rowId, 
        CancellationToken ct = default
    );
    Task<int> CreateAsync(
        int sectionId, 
        string name, 
        int sortOrder, 
        CancellationToken ct = default
    );
    Task UpdateAsync(
        int rowId, 
        string name, 
        int sortOrder, 
        CancellationToken ct = default
    );
    Task DeleteAsync(
        int rowId, 
        CancellationToken ct = default
    );
}
