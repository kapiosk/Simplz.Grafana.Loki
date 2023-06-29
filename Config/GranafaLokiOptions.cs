namespace Simplz.Grafana.Loki.Configs;

public sealed record GranafaLokiOptions
{
    public string BaseAddress { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string APIKey { get; set; } = string.Empty;
}
