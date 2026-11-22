namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Offer
{
    public int OfferId { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime SaleStartAt { get; set; }
    public DateTime? SaleEndAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public Event? Event { get; set; }
}
