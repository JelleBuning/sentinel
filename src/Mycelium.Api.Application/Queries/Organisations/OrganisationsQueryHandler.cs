using Mediator;
using Mycelium.Api.Application.Interfaces;
using Mycelium.Api.Domain.Entities;

namespace Mycelium.Api.Application.Queries.Organisations;

public class OrganisationsQueryHandler(IOrganisationRepository organisationRepository)
    : IRequestHandler<OrganisationsQuery, List<Organisation>>
{
    public ValueTask<List<Organisation>> Handle(OrganisationsQuery request, CancellationToken cancellationToken)
    {
        var organisations = organisationRepository.GetAll();
        return ValueTask.FromResult(organisations);
    }
}
