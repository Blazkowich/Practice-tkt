namespace EPM.TKT.Domain.Models.Event;

public sealed record OfferModel
{
    public int OfferId { get; init; }
    public int EventId { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime SaleStartAt { get; init; }
    public DateTime? SaleEndAt { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
