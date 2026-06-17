using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Domain.Entities;

namespace Mycelium.Api.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Organisation> Organisations { get; init; }
    public DbSet<User> Users { get; init; }
    public DbSet<Device> Devices { get; init; }
    public DbSet<DeviceDetails> DeviceDetails { get; init; }
    public DbSet<DeviceSecurity> DeviceSecurities { get; init; }
    public DbSet<DeviceDisk> DeviceDisks { get; init; }
    public DbSet<DeviceSoftware> DeviceSoftware { get; init; }
}