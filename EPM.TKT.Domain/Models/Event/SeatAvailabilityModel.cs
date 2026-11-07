namespace EPM.TKT.Domain.Models.Event;

public sealed record SeatAvailabilityModel
{
    public long                        EventSeatId  { get; init; }
    public int                         VenueSeatId  { get; init; }
    public int                         SectionId    { get; init; }
    public int?                        RowId        { get; init; }
    public string?                     SeatNumber   { get; init; }
    public SeatStateModel              SeatState    { get; init; } = new();
    public IReadOnlyList<PriceOptionModel> PriceOptions { get; init; } = [];
}
