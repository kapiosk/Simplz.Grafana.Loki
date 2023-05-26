namespace Simplz.Grafana.Loki.Models;

public sealed record LogMessage
{
    public Stream[] streams { get; set; } = Array.Empty<Stream>();
}
