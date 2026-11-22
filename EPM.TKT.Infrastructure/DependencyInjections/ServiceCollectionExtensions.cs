using EPM.TKT.App.Repositories;
using EPM.TKT.App.Services;
using EPM.TKT.Infrastructure.Context;
using EPM.TKT.Infrastructure.Options;
using EPM.TKT.Infrastructure.Repositories;
using EPM.TKT.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

        services.AddOptions<JwtOptions>()
            .BindConfiguration("Jwt")
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtOptions>>().Value);

        // DbContext factory / provider chain
        services.AddTransient<IDbContextFactory, DbContextFactory>();
        services.AddTransient<IDbContextProvider, DbContextProvider>();
        services.AddScoped(sp => sp.GetRequiredService<IDbContextProvider>().GetDbContext());

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<IRowRepository, RowRepository>();
        services.AddScoped<IVenueSeatRepository, VenueSeatRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IOfferRepository, OfferRepository>();
        services.AddScoped<IPriceRepository, PriceRepository>();
        services.AddScoped<IEventSeatRepository, EventSeatRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        // Services
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();

        return services;
    }
}
