namespace EPM.TKT.Domain.Models.Order;

public sealed record OrderModel
{
    public Guid     OrderId              { get; init; }
    public Guid     UserId               { get; init; }
    public int      EventId              { get; init; }
    public Guid     CartId               { get; init; }
    public decimal  TotalAmount          { get; init; }
    public string?  PaymentTransactionId { get; init; }
    public DateTime CreatedAt            { get; init; }
}
