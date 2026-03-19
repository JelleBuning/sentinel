using Mediator;
using Sentinel.Api.Domain.Entities;

namespace Sentinel.Api.Application.Queries.Organisations;

public record OrganisationsQuery : IRequest<List<Organisation>>;
