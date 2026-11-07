namespace EPM.TKT.Domain.Models.Event;

public sealed record PriceOptionModel
{
    public int     PriceId  { get; init; }
    public string  Name     { get; init; } = string.Empty;
    public decimal Amount   { get; init; }
    public string  Currency { get; init; } = string.Empty;
}
