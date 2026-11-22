namespace EPM.TKT.ArchitectureTests;

/// <summary>
/// Single source of truth for every project-specific string in this test suite.
/// To adapt the suite to a different solution, change only this file and update
/// the &lt;ProjectReference&gt; entries in the .csproj — no test file needs touching.
/// </summary>
internal static class ArchitectureConfig
{
    // ── Assembly names ────────────────────────────────────────────────────────
    // Must match the AssemblyName (or project file name) of each referenced project.
    internal const string DomainAssembly         = "EPM.TKT.Domain";
    internal const string AppAssembly            = "EPM.TKT.App";
    internal const string InfrastructureAssembly = "EPM.TKT.Infrastructure";
    internal const string WebApiAssembly         = "EPM.TKT.WebApi";

    // ── Layer namespace roots ─────────────────────────────────────────────────
    internal const string DomainNs         = "EPM.TKT.Domain";
    internal const string AppNs            = "EPM.TKT.App";
    internal const string InfrastructureNs = "EPM.TKT.Infrastructure";
    internal const string ExternalNs       = "EPM.TKT.External";
    internal const string WebApiNs         = "EPM.TKT.WebApi";

    // ── Sub-namespace patterns ────────────────────────────────────────────────
    internal const string DomainModelsNs      = DomainNs         + ".Models";
    internal const string AppUseCasesNs       = AppNs            + ".UseCases";
    internal const string AppRepositoriesNs   = AppNs            + ".Repositories";
    internal const string AppServicesNs       = AppNs            + ".Services";
    internal const string InfraRepositoriesNs = InfrastructureNs + ".Repositories";
    internal const string WebApiControllersNs = WebApiNs         + ".Controllers";
}
