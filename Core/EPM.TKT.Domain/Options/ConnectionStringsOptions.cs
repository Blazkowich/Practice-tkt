namespace EPM.TKT.Domain.Options;

public sealed class ConnectionStringsOptions
{
    public IDictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
}
