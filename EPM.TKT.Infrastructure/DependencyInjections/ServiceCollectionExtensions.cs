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
        this IServiceCollection services)
    {
        services
            .AddOptions<ConnectionStringsOptions>()
            .Configure<IConfiguration>((opt, config) =>
            {
                foreach (var entry in config.GetSection("ConnectionStrings").GetChildren())
                    opt.ConnectionStrings[entry.Key] = entry.Value ?? string.Empty;
            });

        // DbContext factory / provider chain
        services.AddTransient<IDbContextFactory, DbContextFactory>();
        services.AddTransient<IDbContextProvider, DbContextProvider>();
        services.AddScoped(sp => sp.GetRequiredService<IDbContextProvider>().GetDbContext());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Services
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }
}
