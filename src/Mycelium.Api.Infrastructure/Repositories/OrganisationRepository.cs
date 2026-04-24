using Microsoft.EntityFrameworkCore;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Api.Domain.Entities;
using Mycelium.Api.Infrastructure.Persistence;

namespace Mycelium.Api.Infrastructure.Repositories;

public class OrganisationRepository(AppDbContext dbContext) : IOrganisationRepository
{
    public List<Organisation> GetAll()
    {
        var orgs = dbContext.Organisations.Include(x => x.Devices).Include(y => y.Users).ToList();
        return orgs;
    }
}