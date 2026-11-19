namespace EPM.TKT.Domain.Models.Venue;

public sealed record RowModel
{
    public int    RowId     { get; init; }
    public int    SectionId { get; init; }
    public string Name      { get; init; } = string.Empty;
    public int    SortOrder { get; init; }
}
