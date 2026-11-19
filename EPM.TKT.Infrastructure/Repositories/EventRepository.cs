using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class EventRepository(ITKTDbContext context) : IEventRepository
{
    public async Task<IReadOnlyList<EventModel>> GetAllAsync(CancellationToken ct = default)
        => await context.Events
            .AsNoTracking()
            .OrderBy(e => e.EventDate)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<EventModel?> GetByIdAsync(int eventId, CancellationToken ct = default)
        => await context.Events
            .AsNoTracking()
            .Where(e => e.EventId == eventId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<int> CreateAsync(int venueId, string name, string? description, DateTime eventDate, DateTime? doorsOpenAt, byte eventStatusId, CancellationToken ct = default)
    {
        var ev = new Event
        {
            VenueId       = venueId,
            Name          = name,
            Description   = description,
            EventDate     = eventDate,
            DoorsOpenAt   = doorsOpenAt,
            EventStatusId = eventStatusId,
            CreatedAt     = DateTime.UtcNow
        };
        context.Events.Add(ev);
        await context.SaveChangesAsync(ct);
        return ev.EventId;
    }

    public async Task UpdateAsync(int eventId, string name, string? description, DateTime eventDate, DateTime? doorsOpenAt, byte eventStatusId, CancellationToken ct = default)
        => await context.Events
            .Where(e => e.EventId == eventId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Name, name)
                .SetProperty(x => x.Description, description)
                .SetProperty(x => x.EventDate, eventDate)
                .SetProperty(x => x.DoorsOpenAt, doorsOpenAt)
                .SetProperty(x => x.EventStatusId, eventStatusId), ct);

    public async Task DeleteAsync(int eventId, CancellationToken ct = default)
        => await context.Events
            .Where(e => e.EventId == eventId)
            .ExecuteDeleteAsync(ct);

    public async Task<IReadOnlyList<SeatAvailabilityModel>> GetSeatsAsync(
        int eventId, int sectionId, CancellationToken ct = default)
    {
        // Load active offer IDs for this event
        var offerIds = await context.Offers
            .AsNoTracking()
            .Where(o => o.EventId == eventId && o.IsActive)
            .Select(o => o.OfferId)
            .ToListAsync(ct);

        // Load price options scoped to the section (or global = null SectionId)
        var priceOptions = await context.Prices
            .AsNoTracking()
            .Where(p => offerIds.Contains(p.OfferId)
                        && (p.SectionId == null || p.SectionId == sectionId))
            .Select(p => new PriceOptionModel
            {
                PriceId  = p.PriceId,
                Name     = p.PriceCategory != null ? p.PriceCategory.Name : string.Empty,
                Amount   = p.Amount,
                Currency = p.Currency
            })
            .ToListAsync(ct);

        return await context.EventSeats
            .AsNoTracking()
            .Where(es => es.EventId == eventId
                         && es.VenueSeat != null
                         && es.VenueSeat.SectionId == sectionId)
            .MapToSeatAvailability(priceOptions)
            .ToListAsync(ct);
    }
}
