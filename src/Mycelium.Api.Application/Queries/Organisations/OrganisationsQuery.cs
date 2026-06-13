using Mediator;
using Mycelium.Api.Domain.Entities;

namespace Mycelium.Api.Application.Queries.Organisations;

public record OrganisationsQuery : IRequest<List<Organisation>>;
