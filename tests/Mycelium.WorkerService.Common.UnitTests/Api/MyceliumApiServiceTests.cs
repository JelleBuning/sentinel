using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Mycelium.Common.DTO.Device;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.UnitTests.TestSupport;
using NUnit.Framework;

namespace Mycelium.WorkerService.Common.UnitTests.Api;

public class MyceliumApiServiceTests
{
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Id"] = "42" })
            .Build();
    }

    [Test]
    public async Task RegisterDeviceAsync_Success_PostsToRegisterEndpointAndReturnsParsedResponse()
    {
        var handler = new FakeHttpMessageHandler(_ =>
        {
            var body = new StringContent(
                """{"id":1,"organisationId":2,"accessToken":"access","refreshToken":"refresh"}""",
                Encoding.UTF8, "application/json");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = body });
        });
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var service = new MyceliumApiService(client, BuildConfiguration(), NullLogger<MyceliumApiService>.Instance);
        var organisationHash = Guid.NewGuid();

        var result = await service.RegisterDeviceAsync(organisationHash, "MY-DEVICE", CancellationToken.None);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.AccessToken, Is.EqualTo("access"));
        var request = handler.Requests.Single();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(request.RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/register"));
        var requestBody = await request.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(requestBody);
        Assert.That(document.RootElement.GetProperty("Name").GetString(), Is.EqualTo("MY-DEVICE"));
    }

    [Test]
    public async Task RegisterDeviceAsync_FailureResponse_ReturnsNullInsteadOfThrowing()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var service = new MyceliumApiService(client, BuildConfiguration(), NullLogger<MyceliumApiService>.Instance);

        var result = await service.RegisterDeviceAsync(Guid.NewGuid(), "MY-DEVICE", CancellationToken.None);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void PingAsync_FailureResponse_ThrowsBecauseNotCaught()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var service = new MyceliumApiService(client, BuildConfiguration(), NullLogger<MyceliumApiService>.Instance);

        Assert.ThrowsAsync<HttpRequestException>(async () => await service.PingAsync());
    }

    [Test]
    public async Task PingAsync_Success_PostsToDeviceSpecificPingEndpoint()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var service = new MyceliumApiService(client, BuildConfiguration(), NullLogger<MyceliumApiService>.Instance);

        await service.PingAsync();

        var request = handler.Requests.Single();
        Assert.That(request.RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/42/ping"));
    }

    [Test]
    public async Task UpdateDiskInformationAsync_Success_PutsDisksToDisksEndpoint()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var service = new MyceliumApiService(client, BuildConfiguration(), NullLogger<MyceliumApiService>.Instance);

        await service.UpdateDiskInformationAsync([new DiskDto { Name = "C:" }]);

        var request = handler.Requests.Single();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Put));
        Assert.That(request.RequestUri!.AbsolutePath, Is.EqualTo("/api/v1/devices/42/disks"));
    }
}
