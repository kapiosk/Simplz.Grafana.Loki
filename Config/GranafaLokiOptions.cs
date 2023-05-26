namespace Simplz.Grafana.Loki.Configs;

public sealed record GranafaLokiOptions
{
    public string BaseAddress { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string APIKey { get; init; } = string.Empty;
}
