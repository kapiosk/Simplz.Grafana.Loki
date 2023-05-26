namespace Simplz.Grafana.Loki.Models;

public sealed record LokiResponse<T> where T : new()
{
    public string status { get; init; } = string.Empty;
    public T data { get; init; } = new();
}
