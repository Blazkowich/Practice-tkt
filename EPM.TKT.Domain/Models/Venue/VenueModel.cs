namespace EPM.TKT.Domain.Models.Venue;

public sealed record VenueModel
{
    public int     VenueId { get; init; }
    public string  Name    { get; init; } = string.Empty;
    public string? City    { get; init; }
    public string? Country { get; init; }
}
