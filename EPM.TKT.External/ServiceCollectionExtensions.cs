using EPM.TKT.App.Services;
using EPM.TKT.External.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.External;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterExternal(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentServiceClient, StubPaymentServiceClient>();
        return services;
    }
}
