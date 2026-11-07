using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Ticket", Schema = Constants.Tkt)]
internal class Ticket
{
    [Key]
    public Guid TicketId { get; set; }

    public Guid OrderId { get; set; }

    public int EventId { get; set; }

    public long EventSeatId { get; set; }

    public Guid UserId { get; set; }

    public int PriceId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PaidAmount { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public Guid? CancelledByManagerId { get; set; }

    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(EventSeatId))]
    public EventSeat? EventSeat { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(PriceId))]
    public Price? Price { get; set; }
}
