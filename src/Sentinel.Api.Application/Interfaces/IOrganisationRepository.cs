using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Interfaces;

public interface IOrganisationRepository
{
    public List<Organisation> GetAll();
}