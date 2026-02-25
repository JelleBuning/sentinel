using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Sentinel.Api.Application.Mediator.Behaviors;

namespace Sentinel.Api.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatorServices(assembly);
            services.AddValidationServices(assembly);

            return services;
        }

        private void AddMediatorServices(Assembly assembly)
        {
            services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Transient;
            });

            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        private void AddValidationServices(Assembly assembly)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
    }
}