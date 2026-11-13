namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Section
{
    public int SectionId { get; set; }
    public int VenueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public byte SeatTypeId { get; set; }
    public int SortOrder { get; set; }

    public Venue? Venue { get; set; }
    public SeatType? SeatType { get; set; }
}
