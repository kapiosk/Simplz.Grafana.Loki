using System.Text;
using Microsoft.AspNetCore.Http;

namespace Simplz.Grafana.Loki;

public sealed class GrafanaLokiQueryBuilder
{
    private readonly StringBuilder _builder;
    private Dictionary<string, string?> _queryItems;

    public GrafanaLokiQueryBuilder()
    {
        _builder = new();
        _queryItems = new();
    }

    public GrafanaLokiQueryBuilder(string query)
    {
        _builder = new();
        _queryItems = new() { { "query", query } };
    }

    public void SelectApp(string appName)
    {
        _builder.Append(@$"{{app=""{appName}""}} |= ``");
    }

    public void AddStartTime(DateTimeOffset startime)
    {
        _queryItems.Add("start", startime.ToUnixTimeSeconds().ToString());
    }

    public void AddEndTime(DateTimeOffset endTime)
    {
        _queryItems.Add("end", endTime.ToUnixTimeSeconds().ToString());
    }

    public void ExpandAsJson()
    {
        _builder.Append(" | json");
    }

    public void ContainsField(string field)
    {
        _builder.Append($" | {field} != ``");
    }

    public void FilterJsonBy(string field, string value, bool exact = true)
    {
        if (exact)
            _builder.Append($" | {field} = \"{value}\"");
        else
            _builder.Append($" | {field} =~ \"{value}.*\"");
    }

    public QueryString BuildQueryString()
    {
        _queryItems.Add("limit", "1000");
        _queryItems.Add("query", _builder.ToString());
        return QueryString.Create(_queryItems); ;
    }
}
