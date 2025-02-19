using Microsoft.Extensions.DependencyInjection;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Infrastructure.Persistence;

namespace Sentinel.Api.Integration.Tests.Common;

public static class FactoryExtensions
{
    public static HttpClient CreateAuthenticatedClient(this ApiFixture fixture, out SignInUserResponse user)
    {
        var client = fixture.CreateClient();
        user = client.Authenticate().Result;
        return client;
    }
    
    public static async Task<Domain.Entities.Organisation> AddOrganisationAsync(this ApiFixture fixture, Domain.Entities.Organisation organisation)
    {
        var provider = fixture.Services.CreateScope().ServiceProvider;
        await using var dbContext = provider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        dbContext.Organisations.Add(organisation);
        await dbContext.SaveChangesAsync();
        return organisation;
    }
    
    public static async Task AddDeviceAsync(this ApiFixture fixture, Domain.Entities.Device device)
    {
        var provider = fixture.Services.CreateScope().ServiceProvider;
        await using var dbContext = provider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        dbContext.Devices.Add(device);
        await dbContext.SaveChangesAsync();
    }
}