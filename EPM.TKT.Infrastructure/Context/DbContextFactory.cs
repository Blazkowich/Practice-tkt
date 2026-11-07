using EPM.TKT.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EPM.TKT.Infrastructure.Context;

internal class DbContextFactory(IOptions<ConnectionStringsOptions> options) : IDbContextFactory
{
    public ITKTDbContext Create(string connectionStringName)
    {
        if (!options.Value.ConnectionStrings.TryGetValue(connectionStringName, out var connectionString))
            throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        var dbOptions = new DbContextOptionsBuilder<TKTDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new TKTDbContext(dbOptions);
    }
}
