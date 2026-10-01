using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Mycelium.WorkerService.Common.Api;
using Mycelium.WorkerService.Common.DTO;
using Mycelium.WorkerService.Common.Services.Interfaces;
using Mycelium.WorkerService.Common.UnitTests.TestSupport;
using NSubstitute;
using NUnit.Framework;

namespace Mycelium.WorkerService.Common.UnitTests.Api;

public class AuthenticationDelegatingHandlerTests
{
    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AccessToken"] = "initial-access-token",
                ["RefreshToken"] = "initial-refresh-token",
                ["ConnectionStrings:Api"] = "http://localhost"
            })
            .Build();
    }

    [Test]
    public async Task SendAsync_SuccessfulResponse_AttachesBearerTokenAndReturnsResponseUnchanged()
    {
        var configuration = BuildConfiguration();
        var credentialManager = Substitute.For<ICredentialManager>();
        var fakeHandler = new FakeHttpMessageHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));
        var handler = new AuthenticationDelegatingHandler(configuration, credentialManager) { InnerHandler = fakeHandler };
        var client = new HttpClient(handler);

        var response = await client.GetAsync("http://localhost/api/v1/devices");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(fakeHandler.Requests, Has.Count.EqualTo(1));
        Assert.That(fakeHandler.Requests[0].Headers.Authorization?.Scheme, Is.EqualTo("bearer"));
        Assert.That(fakeHandler.Requests[0].Headers.Authorization?.Parameter, Is.EqualTo("initial-access-token"));
    }

    [Test]
    public async Task SendAsync_UnauthorizedWithoutExpiredJwtBody_ReturnsUnauthorizedWithoutRefreshing()
    {
        var configuration = BuildConfiguration();
        var credentialManager = Substitute.For<ICredentialManager>();
        var fakeHandler = new FakeHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Some other error") }));
        var handler = new AuthenticationDelegatingHandler(configuration, credentialManager) { InnerHandler = fakeHandler };
        var client = new HttpClient(handler);

        var response = await client.GetAsync("http://localhost/api/v1/devices");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        Assert.That(fakeHandler.Requests, Has.Count.EqualTo(1));
        await credentialManager.DidNotReceive().SetTokensAsync(Arg.Any<DeviceTokenResponse>());
    }

    [Test]
    public async Task SendAsync_ExpiredJwt_RefreshesTokenAndRetriesOriginalRequest()
    {
        var configuration = BuildConfiguration();
        var credentialManager = Substitute.For<ICredentialManager>();
        var originalRequestCount = 0;

        var fakeHandler = new FakeHttpMessageHandler(request =>
        {
            if (request.RequestUri!.AbsolutePath == "/api/v1/auth/refresh")
            {
                var body = new StringContent("""{"accessToken":"new-access-token","refreshToken":"new-refresh-token"}""", Encoding.UTF8, "application/json");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = body });
            }

            originalRequestCount++;
            if (originalRequestCount == 1)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("Expired JWT") });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        var handler = new AuthenticationDelegatingHandler(configuration, credentialManager) { InnerHandler = fakeHandler };
        var client = new HttpClient(handler);

        var response = await client.GetAsync("http://localhost/api/v1/devices");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(fakeHandler.Requests, Has.Count.EqualTo(3));
        Assert.That(fakeHandler.Requests[2].Headers.Authorization?.Parameter, Is.EqualTo("new-access-token"));
        await credentialManager.Received(1).SetTokensAsync(Arg.Is<DeviceTokenResponse>(
            t => t.AccessToken == "new-access-token" && t.RefreshToken == "new-refresh-token"));
    }
}
