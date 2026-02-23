using Sentinel.Api.Application.DTO.User;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Domain.Entities;
using Sentinel.Api.Infrastructure.Persistence;
using OtpNet;
using Sentinel.Api.Infrastructure.Exceptions;

namespace Sentinel.Api.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
   public async Task Register(RegisterUserDto user)
    {
        var userExists = dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == user.Email.ToLower());
        if (userExists != null)
        {
            throw new ForbiddenException("Email already in use");
        }

        var key = KeyGeneration.GenerateRandomKey(20);
        var base32String = Base32Encoding.ToString(key);

        var organisation = new Organisation
        {
            Hash = Guid.NewGuid(),
            Users = new List<User>
            {
                new()
                {
                    Email = user.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                    TwoFactorToken = base32String,
                }
            }
        };

        await dbContext.Organisations.AddAsync(organisation);
        await dbContext.SaveChangesAsync();
    }

}