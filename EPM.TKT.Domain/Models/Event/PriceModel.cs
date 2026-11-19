namespace EPM.TKT.Domain.Models.Event;

public sealed record PriceModel
{
    public int     PriceId       { get; init; }
    public int     OfferId       { get; init; }
    public int?    SectionId     { get; init; }
    public int?    RowId         { get; init; }
    public string  PriceCategory { get; init; } = string.Empty;
    public decimal Amount        { get; init; }
    public string  Currency      { get; init; } = "USD";
}
