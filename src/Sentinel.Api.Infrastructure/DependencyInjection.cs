using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Sentinel.Api.Application.Interfaces;
using Sentinel.Api.Application.Services;
using Sentinel.Api.Application.Services.Interfaces;
using Sentinel.Api.Infrastructure.Exceptions;
using Sentinel.Api.Infrastructure.Middleware;
using Sentinel.Api.Infrastructure.Persistence;
using Sentinel.Api.Infrastructure.Repositories;
using Serilog;
using Serilog.Events;
using Sentinel.Api.Infrastructure.SignalR;

namespace Sentinel.Api.Infrastructure;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            services
                .AddCors()
                .AddServices()
                .AddPersistence(configuration)
                .AddAuthenticationAndAuthorization(configuration)
                .AddHttpContextAccessor()
                .AddAuthorization()
                .AddSignalR();

            return services;
        }

        private IServiceCollection AddServices()
        {
            services.AddScoped<IJwtTokenGenerator, TokenGenerator>();
            services.AddScoped<IDeviceMessenger, SignalRDeviceMessenger>();
            return services;
        }

        private IServiceCollection AddPersistence(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Database"));
            });

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IOrganisationRepository, OrganisationRepository>();
            
            return services;
        }

        private IServiceCollection AddAuthenticationAndAuthorization(IConfiguration configuration)
        {
            services.AddAuthorization(options =>
                {
                    options.AddPolicy("User", policy => policy.RequireRole("User"));
                    options.AddPolicy("Device", policy => policy.RequireRole("Device"));
                })
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                        
                            var token = context.Request.Headers["Authorization"].ToString();
                            if (token.Contains("bearer"))
                            {
                                var jwtToken = token.Replace("bearer", "", StringComparison.OrdinalIgnoreCase).Trim();
                                if (new JwtSecurityTokenHandler().ReadToken(jwtToken).ValidTo < DateTime.UtcNow)
                                {
                                    await context.Response.WriteAsync("Expired JWT");
                                    return;
                                }
                            }
                            await context.Response.WriteAsync("Invalid JWT");
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                    };
                });

            return services;
        }
    }
}