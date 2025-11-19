using System.Net.Http.Json;
using System.Text.Json;

namespace Sentinel.WorkerService.Common.Extensions;

public static class HttpContentExtensions
{
    public static Task<T?> DeserializeAsync<T>(this HttpContent content, CancellationToken cancellationToken = default, JsonSerializerOptions? serializerOptions = null)
    {
        var options = serializerOptions ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return content.ReadFromJsonAsync<T>(options, cancellationToken);
    }
}