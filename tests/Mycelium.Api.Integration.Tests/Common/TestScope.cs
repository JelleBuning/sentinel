using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mycelium.Api.Auth.Dto;
using Mycelium.Api.EntityFramework.Persistence;
using DeviceEntity = Mycelium.Api.EntityFramework.Entities.Device;
using OrganisationEntity = Mycelium.Api.EntityFramework.Entities.Organisation;

namespace Mycelium.Api.IntegrationTests.Common;

public sealed class TestScope : IAsyncDisposable
{
    private readonly ApiFixture _fixture;
    private readonly IServiceScope _scope;
    private Guid? _organisationHash = null;

    public HttpClient Client { get; private set; }
    public AppDbContext DbContext { get; }
    public ApiFixture Fixture => _fixture;

    public OrganisationEntity Organisation => DbContext.Organisations
        .Include(x => x.Devices)
        .Include(x => x.Users)
        .Single(x => x.Hash == _organisationHash);

    public SignInResponse? User { get; private set; }

    public TestScope()
    {
        _fixture = new ApiFixture();
        _scope = _fixture.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        DbContext.Database.EnsureCreated();
        Client = _fixture.CreateClient();
    }

    public async Task<TestScope> AuthenticateAsUserAsync()
    {
        var (client, user) = await _fixture.CreateAuthenticatedUserAsync();
        _organisationHash = DbContext.Organisations.Single(x => x.Id == user.OrganisationId).Hash;
        Client = client;
        User = user;

        return this;
    }

    public async Task<TestScope> AuthenticateAsDeviceAsync()
    {
        if (_organisationHash == null)
            await AddOrganisationAsync();

        var (client, device) = await _fixture.CreateAuthenticatedDeviceAsync(_organisationHash ?? throw new Exception("organisation is null"));
        Client = client;

        return this;
    }

    public async Task<TestScope> AddOrganisationAsync()
    {
        var hash = Guid.NewGuid();
        await Fixture.AddOrganisationAsync(hash);
        _organisationHash = hash;
        return this;
    }

    public async Task<TestScope> AddDeviceAsync()
    {
        var device = new DeviceEntity
        {
            Name = "Test Device",
            OrganisationId = Organisation?.Id ?? throw new Exception("Organisation is null. Ensure organisation is added."),
        };
        await Fixture.AddDeviceAsync(device);
        return this;
    }

    public async ValueTask DisposeAsync()
    {
        _scope.Dispose();
        await _fixture.DisposeAsync();
    }
}