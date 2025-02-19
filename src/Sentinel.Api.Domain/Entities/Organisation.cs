namespace Sentinel.Api.Domain.Entities;

public sealed class Organisation
{
    public int Id { get; set; }
    public Guid Hash { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();
}
