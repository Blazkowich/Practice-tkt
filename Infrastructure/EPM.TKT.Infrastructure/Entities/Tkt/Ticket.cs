using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Ticket
{
    public Guid TicketId { get; set; }
    public Guid OrderId { get; set; }
    public int EventId { get; set; }
    public long EventSeatId { get; set; }
    public Guid UserId { get; set; }
    public int PriceId { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public Guid? CancelledByManagerId { get; set; }

    public Order? Order { get; set; }
    public Event? Event { get; set; }
    public EventSeat? EventSeat { get; set; }
    public User? User { get; set; }
    public Price? Price { get; set; }
}
