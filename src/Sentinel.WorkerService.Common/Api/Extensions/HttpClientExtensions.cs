using System.Text;
using System.Text.Json;

namespace Sentinel.WorkerService.Common.Api.Extensions;

public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public async Task<HttpResponseMessage> PostAsync(string requestUri, object? payload = null, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUri, content, cancellationToken);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, object payload, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await client.PutAsync(requestUri, content, cancellationToken);
        }
    }
}