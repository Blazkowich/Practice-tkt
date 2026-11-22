using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class DependencyRulesTests
{
    private const string InfrastructureNs = "EPM.TKT.Infrastructure";
    private const string AppNs            = "EPM.TKT.App";
    private const string WebApiNs         = "EPM.TKT.WebApi";
    private const string ExternalNs       = "EPM.TKT.External";

    // ── Domain must be the innermost layer ───────────────────────────────────

    [Test]
    public void Domain_ShouldNot_DependOn_Application()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(AppNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on the Application layer.");
    }

    [Test]
    public void Domain_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on Infrastructure.");
    }

    [Test]
    public void Domain_ShouldNot_DependOn_WebApi()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on the WebApi layer.");
    }

    // ── Application must not reach outward ───────────────────────────────────

    [Test]
    public void Application_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on Infrastructure.");
    }

    [Test]
    public void Application_ShouldNot_DependOn_WebApi()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on the WebApi layer.");
    }

    [Test]
    public void Application_ShouldNot_DependOn_External()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(ExternalNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on External clients.");
    }

    // ── Infrastructure must not reach into the presentation layer ────────────

    [Test]
    public void Infrastructure_ShouldNot_DependOn_WebApi()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .ShouldNot().HaveDependencyOn(WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Infrastructure must not depend on the WebApi layer.");
    }
}
