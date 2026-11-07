namespace EPM.TKT.Infrastructure.Context.Interfaces;

internal interface IDbContextFactory
{
    ITKTDbContext Create(string connectionStringName);
}
