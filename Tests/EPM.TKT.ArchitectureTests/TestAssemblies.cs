using System.Reflection;
using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Infrastructure.DependencyInjections;
using EPM.TKT.WebApi.Controllers;

namespace EPM.TKT.ArchitectureTests;

/// <summary>
/// Assembly anchors — static references ensure each assembly is loaded into the AppDomain
/// before any architecture test runs, so Types.InAssembly() finds all types reliably.
/// </summary>
internal static class TestAssemblies
{
    internal static readonly Assembly Domain         = typeof(Error).Assembly;
    internal static readonly Assembly App            = typeof(ICartRepository).Assembly;
    internal static readonly Assembly Infrastructure = typeof(ServiceCollectionExtensions).Assembly;
    internal static readonly Assembly WebApi         = typeof(AuthController).Assembly;
}
