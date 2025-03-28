using Sentinel.Api.Application.DTO.User;

namespace Sentinel.Api.Application.Interfaces;

public interface IUserRepository
{
    Task Register(RegisterUserDto user);
}