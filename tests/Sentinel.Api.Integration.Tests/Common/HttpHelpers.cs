using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Sentinel.Api.Integration.Tests.Common;

public static class HttpClientExtensions
{
    extension(HttpClient client)
    {
        public async Task<HttpResponseMessage> PostAsync(string requestUri, object? payload = null, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            try
            {
                return await client.PostAsync(requestUri, content, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public async Task<HttpResponseMessage> PutAsync(string requestUri, object payload, CancellationToken cancellationToken = default)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return await client.PutAsync(requestUri, content, cancellationToken);
        }
    }
}

public static class HttpContentExtensions
{
    public static Task<T?> DeserializeAsync<T>(this HttpContent content, CancellationToken cancellationToken = default, JsonSerializerOptions? serializerOptions = null)
    {
        var options = serializerOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return content.ReadFromJsonAsync<T>(options, cancellationToken);
    }
}
