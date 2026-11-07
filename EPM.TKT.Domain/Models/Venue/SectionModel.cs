namespace EPM.TKT.Domain.Models.Venue;

public sealed record SectionModel
{
    public int    SectionId { get; init; }
    public int    VenueId   { get; init; }
    public string Name      { get; init; } = string.Empty;
    public string SeatType  { get; init; } = string.Empty;
}
