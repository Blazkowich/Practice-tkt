using EPM.TKT.Domain.Models.Ticket;

namespace EPM.TKT.App.Repositories;

public interface ITicketRepository
{
    Task<TicketModel?> GetByIdAsync(
        Guid ticketId, 
        CancellationToken ct = default
    );
    Task<IReadOnlyList<TicketModel>> GetByOrderAsync(
        Guid orderId, 
        CancellationToken ct = default
    );
    Task<IReadOnlyList<TicketModel>> GetByUserAsync(
        Guid userId, 
        CancellationToken ct = default
    );
    Task<Guid> CreateAsync(
        Guid orderId, 
        int eventId, 
        long eventSeatId, 
        Guid userId, 
        int priceId, 
        decimal paidAmount, 
        CancellationToken ct = default
    );
    Task CancelAsync(
        Guid ticketId, 
        Guid cancelledByManagerId, 
        CancellationToken ct = default
    );
}
