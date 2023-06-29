namespace Simplz.Grafana.Loki.Models;

public sealed record Result
{
    public List<Stream> result { get; set; } = new();
}
