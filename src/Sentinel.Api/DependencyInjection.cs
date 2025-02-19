using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Sentinel.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.Configure<JsonOptions>(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.DocInclusionPredicate((_, _) => true);
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        services.AddProblemDetails();
        return services;
    }
}