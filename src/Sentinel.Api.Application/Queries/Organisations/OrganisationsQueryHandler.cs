using Mediator;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Queries.Organisations;

public class OrganisationsQueryHandler(IOrganisationRepository organisationRepository)
    : IRequestHandler<OrganisationsQuery, List<Organisation>>
{
    public ValueTask<List<Organisation>> Handle(OrganisationsQuery request, CancellationToken cancellationToken)
    {
        var organisations = organisationRepository.GetAll();
        return ValueTask.FromResult(organisations);
    }
}
