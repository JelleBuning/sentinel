using Mediator;
using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Queries.Organisations.GetAllOrganisations;

public record GetAllOrganisationsQuery : IRequest<List<Organisation>>;
