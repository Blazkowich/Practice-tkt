namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Price
{
    public int PriceId { get; set; }
    public int OfferId { get; set; }
    public int? SectionId { get; set; }
    public int? RowId { get; set; }
    public byte PriceCategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";

    public Offer? Offer { get; set; }
    public Section? Section { get; set; }
    public Row? Row { get; set; }
    public PriceCategory? PriceCategory { get; set; }
}
