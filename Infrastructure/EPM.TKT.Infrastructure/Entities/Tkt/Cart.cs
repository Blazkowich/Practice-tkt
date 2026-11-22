using EPM.TKT.Infrastructure.Entities.Sec;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Cart
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public int EventId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public User? User { get; set; }
    public Event? Event { get; set; }
}
