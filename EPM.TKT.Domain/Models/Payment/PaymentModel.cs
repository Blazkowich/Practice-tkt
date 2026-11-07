namespace EPM.TKT.Domain.Models.Payment;

public sealed record PaymentModel
{
    public Guid    PaymentId   { get; init; }
    public Guid    CartId      { get; init; }
    public string  Status      { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
}
