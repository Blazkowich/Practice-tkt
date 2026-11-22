using NetArchTest.Rules;

namespace EPM.TKT.ArchitectureTests;

[TestFixture]
public sealed class DependencyRulesTests
{
    // ── Domain must be the innermost layer ───────────────────────────────────

    [Test]
    public void Domain_ShouldNot_DependOn_Application()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.AppNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on the Application layer.");
    }

    [Test]
    public void Domain_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on Infrastructure.");
    }

    [Test]
    public void Domain_ShouldNot_DependOn_WebApi()
    {
        var result = Types.InAssembly(TestAssemblies.Domain)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Domain must not depend on the WebApi layer.");
    }

    // ── Application must not reach outward ───────────────────────────────────

    [Test]
    public void Application_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.InfrastructureNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on Infrastructure.");
    }

    [Test]
    public void Application_ShouldNot_DependOn_WebApi()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on the WebApi layer.");
    }

    [Test]
    public void Application_ShouldNot_DependOn_External()
    {
        var result = Types.InAssembly(TestAssemblies.App)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.ExternalNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Application layer must not depend on External clients.");
    }

    // ── Infrastructure must not reach into the presentation layer ────────────

    [Test]
    public void Infrastructure_ShouldNot_DependOn_WebApi()
    {
        var result = Types.InAssembly(TestAssemblies.Infrastructure)
            .ShouldNot().HaveDependencyOn(ArchitectureConfig.WebApiNs)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True, "Infrastructure must not depend on the WebApi layer.");
    }
}
