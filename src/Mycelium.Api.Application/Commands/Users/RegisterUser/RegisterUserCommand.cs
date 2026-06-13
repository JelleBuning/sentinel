using Mediator;

namespace Mycelium.Api.Application.Commands.Users.RegisterUser;

public record RegisterUserCommand(string Email, string Password) : IRequest;
