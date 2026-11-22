using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

/// <summary>
/// Verifies that the layered architecture forms a strict DAG with no feedback edges.
/// Each test checks one direction of a potential cycle between two layers.
/// Together they prove: Domain → App → Infrastructure → WebApi (one-way only).
/// </summary>
[TestFixture]
public sealed class CyclicDependencyTests
{
    private const string DomainNs         = "EPM.TKT.Domain";
    private const string AppNs            = "EPM.TKT.App";
    private const string InfrastructureNs = "EPM.TKT.Infrastructure";
    private const string WebApiNs         = "EPM.TKT.WebApi";

    // ── No feedback from App back to Domain ───────────────────────────────────
    // (Domain → App would create a cycle; Domain must stay pure)

    [Test]
    public void Domain_ShouldNot_Reference_App_CreatingCycle()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(AppNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Cycle detected: Domain → App would prevent App from depending on Domain.");
    }

    // ── No feedback from Infrastructure back to App ───────────────────────────

    [Test]
    public void App_ShouldNot_Reference_Infrastructure_CreatingCycle()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Cycle detected: App → Infrastructure would create a feedback loop.");
    }

    // ── No feedback from WebApi back to Infrastructure ────────────────────────

    [Test]
    public void Infrastructure_ShouldNot_Reference_WebApi_CreatingCycle()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .ShouldNot().HaveDependencyOn(WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Cycle detected: Infrastructure → WebApi would create a feedback loop.");
    }

    // ── Presentation must not reach back into Infrastructure or App ───────────

    [Test]
    public void WebApi_ShouldNot_Reference_Infrastructure_Directly()
    {
        // WebApi should speak to the App layer only; Infrastructure is wired by DI.
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace(WebApiNs + ".Controllers")
            .ShouldNot().HaveDependencyOn(InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Controllers must not take direct dependencies on Infrastructure — use the App layer.");
    }

    [Test]
    public void WebApi_Controllers_ShouldNot_Reference_Domain_Repositories()
    {
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace(WebApiNs + ".Controllers")
            .ShouldNot().HaveDependencyOn(AppNs + ".Repositories")
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Controllers must not depend on repository interfaces directly.");
    }
}
