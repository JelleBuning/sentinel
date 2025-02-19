using System.Text;
using System.Text.Json;

namespace Sentinel.WorkerService.Common.Api.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, object? payload = null, CancellationToken cancellationToken = default)
    {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return await client.PostAsync(requestUri, content, cancellationToken);
    }
    
    public static async Task<HttpResponseMessage> PutAsync(this HttpClient client, string requestUri, object payload, CancellationToken cancellationToken = default)
    {
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        return await client.PutAsync(requestUri, content, cancellationToken);
    }
}