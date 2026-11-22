using EPM.TKT.App.Services;

namespace EPM.TKT.External.Services;

/// <summary>
/// Development stub — returns a deterministic payment ID derived from the order ID.
/// Replace with a real HTTP client in production.
/// </summary>
internal sealed class StubPaymentServiceClient : IPaymentServiceClient
{
    public Task<Guid> InitiatePaymentAsync(Guid orderId, decimal amount, CancellationToken cancellationToken = default)
        => Task.FromResult(orderId); // stub: payment ID == order ID for dev
}
