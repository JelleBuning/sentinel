
namespace Mycelium.Api.Organisations.GetAll.v1;

public sealed class OrganisationDto
{
    public int Id { get; set; }
    public Guid Hash { get; set; }
    public List<OrganisationUserDto> Users { get; set; } = [];
    public List<OrganisationDeviceDto> Devices { get; set; } = [];
}

public sealed class OrganisationUserDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
}

public sealed class OrganisationDeviceDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime LastActive { get; set; }
}
