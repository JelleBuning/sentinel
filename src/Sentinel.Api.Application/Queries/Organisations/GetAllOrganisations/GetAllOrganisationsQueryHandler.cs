using Mediator;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Queries.Organisations.GetAllOrganisations;

public class GetAllOrganisationsQueryHandler(IOrganisationRepository organisationRepository)
    : IRequestHandler<GetAllOrganisationsQuery, List<Organisation>>
{
    public ValueTask<List<Organisation>> Handle(GetAllOrganisationsQuery request, CancellationToken cancellationToken)
    {
        var organisations = organisationRepository.GetAll();
        return ValueTask.FromResult(organisations);
    }
}
