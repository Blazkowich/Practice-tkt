namespace EPM.TKT.Domain.Models.Venue;

public sealed record VenueSeatModel
{
    public int VenueSeatId { get; init; }
    public int SectionId { get; init; }
    public int? RowId { get; init; }
    public string? SeatNumber { get; init; }
    public int SortOrder { get; init; }
}
