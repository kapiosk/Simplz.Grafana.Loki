namespace Simplz.Grafana.Loki.Models;

public sealed record LogItem
{
    public LogItem(Dictionary<string, string> data, DateTime timestamp)
    {
        Data = data;
        Timestamp = timestamp;
    }

    public Dictionary<string, string> Data { get; init; }
    public DateTime Timestamp { get; init; }
}
