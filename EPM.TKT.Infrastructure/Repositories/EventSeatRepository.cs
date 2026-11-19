using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class EventSeatRepository(ITKTDbContext context) : IEventSeatRepository
{
    public async Task<IReadOnlyList<EventSeatModel>> GetAllAsync(int eventId, CancellationToken ct = default)
        => await context.EventSeats
            .AsNoTracking()
            .Where(es => es.EventId == eventId)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<EventSeatModel?> GetByIdAsync(long eventSeatId, CancellationToken ct = default)
        => await context.EventSeats
            .AsNoTracking()
            .Where(es => es.EventSeatId == eventSeatId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<long> CreateAsync(int eventId, int venueSeatId, int? priceId, CancellationToken ct = default)
    {
        var seat = new EventSeat
        {
            EventId     = eventId,
            VenueSeatId = venueSeatId,
            PriceId     = priceId,
            SeatStateId = 1, // Available
            CreatedAt   = DateTime.UtcNow
        };
        context.EventSeats.Add(seat);
        await context.SaveChangesAsync(ct);
        return seat.EventSeatId;
    }

    public async Task HoldAsync(long eventSeatId, Guid userId, DateTime holdUntil, CancellationToken ct = default)
        => await context.EventSeats
            .Where(es => es.EventSeatId == eventSeatId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.SeatStateId, (byte)2) // Booked
                .SetProperty(x => x.HeldByUserId, userId)
                .SetProperty(x => x.HoldUntil, holdUntil), ct);

    public async Task ReleaseAsync(long eventSeatId, CancellationToken ct = default)
        => await context.EventSeats
            .Where(es => es.EventSeatId == eventSeatId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.SeatStateId, (byte)1) // Available
                .SetProperty(x => x.HeldByUserId, (Guid?)null)
                .SetProperty(x => x.HoldUntil, (DateTime?)null), ct);

    public async Task UpdateStateAsync(long eventSeatId, byte seatStateId, CancellationToken ct = default)
        => await context.EventSeats
            .Where(es => es.EventSeatId == eventSeatId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.SeatStateId, seatStateId), ct);

    public async Task DeleteAsync(long eventSeatId, CancellationToken ct = default)
        => await context.EventSeats
            .Where(es => es.EventSeatId == eventSeatId)
            .ExecuteDeleteAsync(ct);
}
