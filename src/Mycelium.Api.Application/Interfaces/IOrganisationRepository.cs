using Mycelium.Api.Domain.Entities;

namespace Mycelium.Api.Application.Interfaces;

public interface IOrganisationRepository
{
    public List<Organisation> GetAll();
}