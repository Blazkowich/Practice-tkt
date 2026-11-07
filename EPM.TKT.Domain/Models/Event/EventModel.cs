namespace EPM.TKT.Domain.Models.Event;

public sealed record EventModel
{
    public int       EventId     { get; init; }
    public int       VenueId     { get; init; }
    public string    Name        { get; init; } = string.Empty;
    public string?   Description { get; init; }
    public DateTime  EventDate   { get; init; }
    public DateTime? DoorsOpenAt { get; init; }
    public string    EventStatus { get; init; } = string.Empty;
}
