using System.Text;
using Mycelium.WorkerService.Common.Api.Extensions;
using NUnit.Framework;

namespace Mycelium.WorkerService.Common.UnitTests.Api.Extensions;

public class HttpContentExtensionsTests
{
    private sealed class Sample
    {
        public required string AccessToken { get; set; }
    }

    [Test]
    public async Task DeserializeAsync_CamelCaseJson_MapsToPascalCaseProperty()
    {
        var content = new StringContent("""{"accessToken":"abc123"}""", Encoding.UTF8, "application/json");

        var result = await content.DeserializeAsync<Sample>();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.AccessToken, Is.EqualTo("abc123"));
    }

    [Test]
    public async Task DeserializeAsync_NullJson_ReturnsNull()
    {
        var content = new StringContent("null", Encoding.UTF8, "application/json");

        var result = await content.DeserializeAsync<Sample>();

        Assert.That(result, Is.Null);
    }
}
