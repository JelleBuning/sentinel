using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Sentinel.Api.Infrastructure.Persistence;

namespace Sentinel.Api.Integration.Tests.Common;

public class ApiFixture : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("test");
        var root = new InMemoryDatabaseRoot();
        
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("SentinelDatabase", root));
        });
        return base.CreateHost(builder);
    }
}