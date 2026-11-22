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
    // ── No feedback from App back to Domain ───────────────────────────────────

    [Test]
    public void Domain_ShouldNot_Reference_App_CreatingCycle()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.AppNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Cycle detected: Domain → App would prevent App from depending on Domain.");
    }

    // ── No feedback from Infrastructure back to App ───────────────────────────

    [Test]
    public void App_ShouldNot_Reference_Infrastructure_CreatingCycle()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Cycle detected: App → Infrastructure would create a feedback loop.");
    }

    // ── No feedback from WebApi back to Infrastructure ────────────────────────

    [Test]
    public void Infrastructure_ShouldNot_Reference_WebApi_CreatingCycle()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Cycle detected: Infrastructure → WebApi would create a feedback loop.");
    }

    // ── Presentation must not reach back into Infrastructure or App ───────────

    [Test]
    public void WebApi_ShouldNot_Reference_Infrastructure_Directly()
    {
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace(ArchitectureConfig.WebApiControllersNs)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Controllers must not take direct dependencies on Infrastructure — use the App layer.");
    }

    [Test]
    public void WebApi_Controllers_ShouldNot_Reference_Domain_Repositories()
    {
        var result = Types.InAssembly(TestAssemblies.WebApi)
            .That().ResideInNamespace(ArchitectureConfig.WebApiControllersNs)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.AppRepositoriesNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            "Controllers must not depend on repository interfaces directly.");
    }
}
