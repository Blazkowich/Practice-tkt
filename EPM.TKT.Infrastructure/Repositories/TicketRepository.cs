using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Ticket;
using EPM.TKT.Infrastructure.Entities.Mappings.Tkt;
using EPM.TKT.Infrastructure.Entities.Tkt;
using Microsoft.EntityFrameworkCore;

namespace EPM.TKT.Infrastructure.Repositories;

internal sealed class TicketRepository(ITKTDbContext context) : ITicketRepository
{
    public async Task<TicketModel?> GetByIdAsync(Guid ticketId, CancellationToken ct = default)
        => await context.Tickets
            .AsNoTracking()
            .Where(t => t.TicketId == ticketId)
            .MapToDomainModel()
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<TicketModel>> GetByOrderAsync(Guid orderId, CancellationToken ct = default)
        => await context.Tickets
            .AsNoTracking()
            .Where(t => t.OrderId == orderId)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TicketModel>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await context.Tickets
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.IssuedAt)
            .MapToDomainModel()
            .ToListAsync(ct);

    public async Task<Guid> CreateAsync(Guid orderId, int eventId, long eventSeatId, Guid userId, int priceId, decimal paidAmount, CancellationToken ct = default)
    {
        var ticket = new Ticket
        {
            OrderId     = orderId,
            EventId     = eventId,
            EventSeatId = eventSeatId,
            UserId      = userId,
            PriceId     = priceId,
            PaidAmount  = paidAmount,
            IssuedAt    = DateTime.UtcNow
        };
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync(ct);
        return ticket.TicketId;
    }

    public async Task CancelAsync(Guid ticketId, Guid cancelledByManagerId, CancellationToken ct = default)
        => await context.Tickets
            .Where(t => t.TicketId == ticketId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.CancelledAt, DateTime.UtcNow)
                .SetProperty(x => x.CancelledByManagerId, cancelledByManagerId), ct);
}
