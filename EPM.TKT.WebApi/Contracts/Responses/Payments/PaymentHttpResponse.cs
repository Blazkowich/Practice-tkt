namespace EPM.TKT.WebApi.Contracts.Responses.Payments;

public sealed record PaymentHttpResponse(Guid PaymentId, Guid CartId, string Status, decimal TotalAmount);
