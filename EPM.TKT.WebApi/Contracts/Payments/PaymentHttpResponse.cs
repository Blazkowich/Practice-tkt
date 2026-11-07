namespace EPM.TKT.WebApi.Contracts.Payments;

public sealed record PaymentHttpResponse(Guid PaymentId, Guid CartId, string Status, decimal TotalAmount);
