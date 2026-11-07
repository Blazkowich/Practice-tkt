namespace EPM.TKT.Domain.Models.Event;

public sealed record SeatStateModel
{
    public byte   Id   { get; init; }
    public string Name { get; init; } = string.Empty;
}
