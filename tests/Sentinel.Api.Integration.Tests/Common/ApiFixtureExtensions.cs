using Microsoft.Extensions.DependencyInjection;
using Sentinel.Api.Application.DTO.Device;
using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Infrastructure.Persistence;

namespace Sentinel.Api.Integration.Tests.Common;

public static class ApiFixtureExtensions
{
    extension(ApiFixture fixture)
    {
        public AppDbContext GetDbContext()
        {
            var provider = fixture.Services.CreateScope().ServiceProvider;
            var dbContext = provider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
            return dbContext;
        }

        public async Task<(HttpClient client, SignInUserResponse user)> CreateAuthenticatedUserAsync()
        {
            var client = fixture.CreateClient();
            var user = await client.AuthenticateUserAsync();
            return (client, user);
        }

        public async Task<(HttpClient client, DeviceTokenResponse device)> CreateAuthenticatedDeviceAsync(Guid organisationHash)
        {
            var client = fixture.CreateClient();
            var device = await client.RegisterDeviceAsync(organisationHash);
            return (client, device);
        }

        public async Task<Domain.Entities.Organisation> AddOrganisationAsync(Guid organisationHash)
        {
            var provider = fixture.Services.CreateScope().ServiceProvider;
            await using var dbContext = provider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var organisation = new Domain.Entities.Organisation
            {
                Hash = organisationHash
            };
            dbContext.Organisations.Add(organisation);
            await dbContext.SaveChangesAsync();
            return organisation;
        }

        public async Task AddDeviceAsync(Domain.Entities.Device device)
        {
            var provider = fixture.Services.CreateScope().ServiceProvider;
            await using var dbContext = provider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        
            dbContext.Devices.Add(device);
            await dbContext.SaveChangesAsync();
        }
    }
}