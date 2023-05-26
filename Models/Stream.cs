namespace Simplz.Grafana.Loki.Models;

public sealed record Stream
{
    public Dictionary<string, string> stream { get; set; } = new();
    public List<string[]> values { get; set; } = new();
}
