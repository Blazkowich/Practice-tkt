namespace EPM.TKT.Domain.Models.Cart;

public sealed record CartItemModel
{
    public long    EventSeatId { get; init; }
    public int     VenueSeatId { get; init; }
    public string? SeatNumber  { get; init; }
    public string? RowName     { get; init; }
    public string  SectionName { get; init; } = string.Empty;
    public int     PriceId     { get; init; }
    public string  PriceName   { get; init; } = string.Empty;
    public decimal Amount      { get; init; }
    public string  Currency    { get; init; } = string.Empty;
}
