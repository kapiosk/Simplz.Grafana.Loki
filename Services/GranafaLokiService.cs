using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Simplz.Grafana.Loki.Models;

namespace Simplz.Grafana.Loki.Services;

public sealed class GranafaLokiService
{
    private readonly HttpClient _httpClient;

    public GranafaLokiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LokiResponse<Result>?> GetLogsAsync(string app, DateTimeOffset startime, DateTimeOffset? endTime = null, CancellationToken token = default)
    {
        GrafanaLokiQueryBuilder queryBuilder = new();
        queryBuilder.SelectApp(app);
        queryBuilder.AddStartTime(startime);
        if (endTime.HasValue)
            queryBuilder.AddEndTime(endTime.Value);
        queryBuilder.ExpandAsJson();
        return await GetLogsAsync<LokiResponse<Result>>(queryBuilder, token);
    }

    public async Task<T?> GetLogsAsync<T>(GrafanaLokiQueryBuilder queryBuilder, CancellationToken token = default)
    {
        return await GetLogsAsync<T>(queryBuilder.BuildQueryString(), token);
    }

    public async Task<IEnumerable<Models.LogItem>> GetLogItemsAsync(GrafanaLokiQueryBuilder queryBuilder, CancellationToken token = default)
    {
        return (await GetLogsAsync<LokiResponse<Result>>(queryBuilder.BuildQueryString(), token)).ConvertToLogItems();
    }

    public async Task<LokiResponse<Result>?> GetLogsAsync(GrafanaLokiQueryBuilder queryBuilder, CancellationToken token = default)
    {
        return await GetLogsAsync<LokiResponse<Result>>(queryBuilder.BuildQueryString(), token);
    }

    public async Task<T?> GetLogsAsync<T>(QueryString queryString, CancellationToken token = default)
    {
        if (typeof(T) == typeof(string))
            return (T)Convert.ChangeType(await _httpClient.GetStringAsync($"/loki/api/v1/query_range{queryString}", token), typeof(T));
        else
            return await _httpClient.GetFromJsonAsync<T>($"/loki/api/v1/query_range{queryString}", token);
    }

    public async Task<LokiResponse<List<string>>?> GetLabelsAsync(DateTimeOffset startime, DateTimeOffset? endTime = null, CancellationToken token = default)
    {
        List<KeyValuePair<string, string?>> queryItems = new();
        queryItems.Add(new("start", startime.ToUnixTimeSeconds().ToString()));
        if (endTime.HasValue)
            queryItems.Add(new("end", endTime.Value.ToUnixTimeSeconds().ToString()));
        QueryString queryString = QueryString.Create(queryItems);
        return await _httpClient.GetFromJsonAsync<LokiResponse<List<string>>>($"/loki/api/v1/labels{queryString}", token);
    }

    public async Task<LokiResponse<List<string>>?> GetLabelValuesAsync(string label, DateTimeOffset startime, DateTimeOffset? endTime = null, CancellationToken token = default)
    {
        List<KeyValuePair<string, string?>> queryItems = new();
        queryItems.Add(new("start", startime.ToUnixTimeSeconds().ToString()));
        if (endTime.HasValue)
            queryItems.Add(new("end", endTime.Value.ToUnixTimeSeconds().ToString()));
        QueryString queryString = QueryString.Create(queryItems);
        return await _httpClient.GetFromJsonAsync<LokiResponse<List<string>>>($"/loki/api/v1/label/{label}/values{queryString}", token);
    }

    //TODO: Bulk write logs
    public async Task<string?> WriteLogAsync(string logMessage, string appName, DateTime? timestamp = null, CancellationToken token = default)
    {
        if (!timestamp.HasValue)
            timestamp = DateTime.UtcNow;
        LogMessage logData = new()
        {
            streams = new Models.Stream[]
            {
                    new ()
                    {
                        stream = new() { { "app", appName } },
                        values = new() { new[] { timestamp.Value.GetEpoch() , logMessage } }
                    },
            }
        };
        StringContent content = new(System.Text.Json.JsonSerializer.Serialize(logData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/loki/api/v1/push", content);
        return await response.Content.ReadAsStringAsync();
    }
}
