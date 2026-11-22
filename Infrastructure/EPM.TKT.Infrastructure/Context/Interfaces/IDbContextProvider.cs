namespace EPM.TKT.Infrastructure.Context.Interfaces;

internal interface IDbContextProvider
{
    public ITKTDbContext GetDbContext();
}
