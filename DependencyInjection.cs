using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simplz.Grafana.Loki.Configs;
using Simplz.Grafana.Loki.Services;

namespace Simplz.Grafana.Loki;

public static class DependencyInjection
{
    public static IServiceCollection AddGranafaLokiService(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        var options = configurationSection.Get<GranafaLokiOptions>();
        if (options is not null)
            services.AddHttpClient<GranafaLokiService>(
                    o =>
                    {
                        o.BaseAddress = new(options.BaseAddress);
                        var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Username}:{options.APIKey}"));
                        o.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.All,
                    });
        return services;
    }
}
