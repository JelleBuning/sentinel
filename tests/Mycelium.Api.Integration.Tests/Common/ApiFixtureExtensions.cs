using Microsoft.Extensions.DependencyInjection;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.EntityFramework.Persistence;
using Mycelium.Common.DTO.Device;
using DeviceEntity = Mycelium.Api.EntityFramework.Entities.Device;
using OrganisationEntity = Mycelium.Api.EntityFramework.Entities.Organisation;

namespace Mycelium.Api.IntegrationTests.Common;

public static class ApiFixtureExtensions
{
    extension(ApiFixture fixture)
    {
        public async Task<(HttpClient client, SignInResponse user)> CreateAuthenticatedUserAsync()
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

        public async Task<OrganisationEntity> AddOrganisationAsync(Guid organisationHash)
        {
            using var scope = fixture.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            var organisation = new OrganisationEntity
            {
                Hash = organisationHash
            };
            dbContext.Organisations.Add(organisation);
            await dbContext.SaveChangesAsync();
            return organisation;
        }

        public async Task AddDeviceAsync(DeviceEntity device)
        {
            using var scope = fixture.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            dbContext.Devices.Add(device);
            await dbContext.SaveChangesAsync();
        }
    }
}
