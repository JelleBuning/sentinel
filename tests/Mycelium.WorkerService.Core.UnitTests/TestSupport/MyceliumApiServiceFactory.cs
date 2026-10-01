using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.WorkerService.Common.Api;

namespace Mycelium.WorkerService.Core.UnitTests.TestSupport;

internal static class MyceliumApiServiceFactory
{
    public static MyceliumApiService CreateSucceeding(out FakeHttpMessageHandler handler)
    {
        handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Id"] = "1" })
            .Build();
        return new MyceliumApiService(client, configuration, NullLogger<MyceliumApiService>.Instance);
    }
}
