using System.Text.Json;
using Mycelium.WorkerService.Common.Api.Extensions;
using Mycelium.WorkerService.Common.UnitTests.TestSupport;
using NUnit.Framework;

namespace Mycelium.WorkerService.Common.UnitTests.Api.Extensions;

public class HttpClientExtensionsTests
{
    [Test]
    public async Task PostAsync_SerializesPayloadAsJsonBody()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

        var response = await client.PostAsync("/api/v1/things", new { Name = "test" });

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(handler.Requests, Has.Count.EqualTo(1));
        var request = handler.Requests.Single();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Post));
        var body = await request.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.That(document.RootElement.GetProperty("Name").GetString(), Is.EqualTo("test"));
        Assert.That(request.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
    }

    [Test]
    public async Task PostAsync_NoPayload_SerializesNullBody()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

        await client.PostAsync("/api/v1/ping");

        var body = await handler.Requests.Single().Content!.ReadAsStringAsync();
        Assert.That(body, Is.EqualTo("null"));
    }

    [Test]
    public async Task PutAsync_SerializesPayloadAsJsonBody()
    {
        var handler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)));
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

        var response = await client.PutAsync("/api/v1/things/1", new { Name = "updated" });

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        var request = handler.Requests.Single();
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Put));
        var body = await request.Content!.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);
        Assert.That(document.RootElement.GetProperty("Name").GetString(), Is.EqualTo("updated"));
    }
}
