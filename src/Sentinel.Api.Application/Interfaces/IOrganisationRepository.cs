using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Interfaces
{
    public interface IOrganisationRepository : IRepository<Organisation>
    {
        public List<Organisation> GetAll();
    }
}
