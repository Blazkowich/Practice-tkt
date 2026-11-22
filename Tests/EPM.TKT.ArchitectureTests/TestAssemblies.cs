using System.Reflection;

namespace EPM.TKT.ArchitectureTests;

/// <summary>
/// Resolves each layer's Assembly by name, using ArchitectureConfig as the single
/// source of truth. No typeof() anchors — removing all direct type dependencies
/// from specific projects makes this file project-agnostic.
/// </summary>
internal static class TestAssemblies
{
    internal static readonly Assembly Domain         = Load(ArchitectureConfig.DomainAssembly);
    internal static readonly Assembly App            = Load(ArchitectureConfig.AppAssembly);
    internal static readonly Assembly Infrastructure = Load(ArchitectureConfig.InfrastructureAssembly);
    internal static readonly Assembly WebApi         = Load(ArchitectureConfig.WebApiAssembly);

    /// <summary>
    /// Returns an already-loaded assembly when available (project reference brought it in),
    /// otherwise loads it by name from the current probe path.
    /// </summary>
    private static Assembly Load(string assemblyName)
    {
        var already = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == assemblyName);

        return already ?? Assembly.Load(new AssemblyName(assemblyName));
    }
}
