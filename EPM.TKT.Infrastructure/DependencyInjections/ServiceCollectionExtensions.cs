using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Infrastructure.Context;
using EPM.TKT.Infrastructure.Context.Interfaces;
using EPM.TKT.Infrastructure.Options;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EPM.TKT.Infrastructure.DependencyInjections;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Connection strings options — binds "ConnectionStrings" section from appsettings
        services.Configure<ConnectionStringsOptions>(opt =>
        {
            foreach (var entry in configuration.GetSection("ConnectionStrings").GetChildren())
                opt.ConnectionStrings[entry.Key] = entry.Value ?? string.Empty;
        });

        // DbContext factory / provider chain
        services.AddTransient<IDbContextFactory, DbContextFactory>();
        services.AddTransient<IDbContextProvider, DbContextProvider>();
        services.AddScoped(sp => sp.GetRequiredService<IDbContextProvider>().GetDbContext());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }
}
