namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Event
{
    public int EventId { get; set; }
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public DateTime? DoorsOpenAt { get; set; }
    public byte EventStatusId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Venue? Venue { get; set; }
    public EventStatus? EventStatus { get; set; }
}
