namespace EPM.TKT.App.Services;

public interface IPaymentServiceClient
{
    /// <summary>
    /// Initiates a payment with the external Payment Service.
    /// Returns the external payment transaction ID.
    /// </summary>
    Task<Guid> InitiatePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default);
}
