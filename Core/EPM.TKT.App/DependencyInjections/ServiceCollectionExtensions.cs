using EPM.TKT.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.App.DependencyInjections;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplication(
        this IServiceCollection services)
    {
        return services.AddMediator<AppAssemblyMarker>();
    }
}
