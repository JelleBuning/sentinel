using Microsoft.EntityFrameworkCore;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Domain.Entities;
using Sentinel.Api.Infrastructure.Persistence;

namespace Sentinel.Api.Infrastructure.Repositories
{
    public class OrganisationRepository(AppDbContext dbContext) : IOrganisationRepository
    {
        public List<Organisation> GetAll()
        {
            var orgs = dbContext.Organisations.Include(x => x.Devices).Include(y => y.Users).ToList();
            return orgs;
        }
    }
}
