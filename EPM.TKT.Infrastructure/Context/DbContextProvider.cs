using EPM.TKT.Infrastructure.Context.Interfaces;

namespace EPM.TKT.Infrastructure.Context;

internal class DbContextProvider(
    IDbContextFactory dbContextFactory) : IDbContextProvider
{
    public ITKTDbContext GetDbContext()
    {
        var dbName = "TKT"; // for now we have only one db, but in future we can have multiple dbs, so we will get the db name from the configuration or from the request context

        return dbContextFactory.Create(dbName);
    }
}