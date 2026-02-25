using System.Net;
using NUnit.Framework;

namespace Sentinel.Api.Integration.Tests.Common;

public static class HttpResponseAssertions
{
    extension(HttpResponseMessage response)
    {
        public void ShouldBeOk()
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), 
                $"Expected OK but got {response.StatusCode}");
        }

        public void ShouldBeUnauthorized()
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized),
                $"Expected Unauthorized but got {response.StatusCode}");
        }

        public void ShouldBeBadRequest()
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest),
                $"Expected BadRequest but got {response.StatusCode}");
        }

        public void ShouldBeForbidden()
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden),
                $"Expected Forbidden but got {response.StatusCode}");
        }

        public async Task<T> ShouldDeserializeTo<T>()
        {
            var content = await response.Content.DeserializeAsync<T>();
            Assert.That(content, Is.Not.Null, $"Response content could not be deserialized to {typeof(T).Name}");
            return content!;
        }
    }
}
