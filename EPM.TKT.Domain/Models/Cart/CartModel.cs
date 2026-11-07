namespace EPM.TKT.Domain.Models.Cart;

public sealed record CartModel
{
    public Guid                        CartId      { get; init; }
    public int                         EventId     { get; init; }
    public IReadOnlyList<CartItemModel> Items       { get; init; } = [];
    public decimal                     TotalAmount { get; init; }
}
