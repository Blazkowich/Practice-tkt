using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Order
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public int EventId { get; set; }
    public Guid CartId { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Event? Event { get; set; }
    public Cart? Cart { get; set; }
}
