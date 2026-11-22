using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class RowRepository(ITKTDbContext context) : IRowRepository
{
    public async Task<IReadOnlyList<RowModel>> GetAllAsync(
        int sectionId,
        CancellationToken ct = default
    ) => await context.Rows
            .AsNoTracking()
            .Where(r => r.SectionId == sectionId)
            .OrderBy(r => r.SortOrder)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<RowModel?> GetByIdAsync(
        int rowId,
        CancellationToken ct = default
    ) => await context.Rows
            .AsNoTracking()
            .Where(r => r.RowId == rowId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(
        int sectionId,
        string name,
        int sortOrder,
        CancellationToken ct = default
    )
    {
        var row = new Row
        {
            SectionId = sectionId,
            Name = name,
            SortOrder = sortOrder
        };
        context.Rows.Add(row);
        await context.SaveChangesAsync(ct);
        return row.RowId;
    }

    public async Task UpdateAsync(
        int rowId,
        string name,
        int sortOrder,
        CancellationToken ct = default
    )
        => await context.Rows
            .Where(r => r.RowId == rowId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Name, name)
                .SetProperty(x => x.SortOrder, sortOrder), ct);

    public async Task DeleteAsync(
        int rowId,
        CancellationToken ct = default
    ) => await context.Rows
            .Where(r => r.RowId == rowId)
            .ExecuteDeleteAsync(ct);
}
