using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mycelium.Api.EntityFramework.Persistence;
using NSubstitute;

namespace Mycelium.Api.Devices.UnitTests;

internal static class TestDbContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}

internal static class TestHttpContextAccessorFactory
{
    public static IHttpContextAccessor ForDevice(int deviceId)
    {
        var identity = new ClaimsIdentity([new Claim("Id", deviceId.ToString())]);
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(httpContext);
        return accessor;
    }

    public static IHttpContextAccessor Anonymous()
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(new DefaultHttpContext());
        return accessor;
    }
}
