namespace Simplz.Grafana.Loki.Models;

public sealed record LokiResponse<T> where T : new()
{
    public string status { get; set; } = string.Empty;
    public T data { get; set; } = new();
}
