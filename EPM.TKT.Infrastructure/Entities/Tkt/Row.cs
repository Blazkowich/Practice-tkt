namespace EPM.TKT.Infrastructure.Entities.Tkt;

internal class Row
{
    public int RowId { get; set; }
    public int SectionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public Section? Section { get; set; }
}
