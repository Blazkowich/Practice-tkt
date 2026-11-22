using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class VenueSeatRepository(ITKTDbContext context) : IVenueSeatRepository
{
    public async Task<IReadOnlyList<VenueSeatModel>> GetAllAsync(int sectionId, CancellationToken ct = default)
        => await context.VenueSeats
            .AsNoTracking()
            .Where(vs => vs.SectionId == sectionId)
            .OrderBy(vs => vs.SortOrder)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<VenueSeatModel?> GetByIdAsync(int venueSeatId, CancellationToken ct = default)
        => await context.VenueSeats
            .AsNoTracking()
            .Where(vs => vs.VenueSeatId == venueSeatId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(int sectionId, int? rowId, string? seatNumber, int sortOrder, CancellationToken ct = default)
    {
        var seat = new VenueSeat
        {
            SectionId = sectionId,
            RowId = rowId,
            SeatNumber = seatNumber,
            SortOrder = sortOrder
        };
        context.VenueSeats.Add(seat);
        await context.SaveChangesAsync(ct);
        return seat.VenueSeatId;
    }

    public async Task UpdateAsync(int venueSeatId, string? seatNumber, int sortOrder, CancellationToken ct = default)
        => await context.VenueSeats
            .Where(vs => vs.VenueSeatId == venueSeatId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.SeatNumber, seatNumber)
                .SetProperty(x => x.SortOrder, sortOrder), ct);

    public async Task DeleteAsync(int venueSeatId, CancellationToken ct = default)
        => await context.VenueSeats
            .Where(vs => vs.VenueSeatId == venueSeatId)
            .ExecuteDeleteAsync(ct);
}
