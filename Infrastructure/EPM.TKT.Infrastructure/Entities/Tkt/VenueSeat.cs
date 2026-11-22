namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class VenueSeat
{
    public int VenueSeatId { get; set; }
    public int SectionId { get; set; }
    public int? RowId { get; set; }
    public string? SeatNumber { get; set; }
    public int SortOrder { get; set; }

    public Section? Section { get; set; }
    public Row? Row { get; set; }
}
