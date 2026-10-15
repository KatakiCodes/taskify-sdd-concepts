using System.Reflection;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using TaskFy.Application.Cqrs;

namespace TaskFy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] additionalAssemblies)
    {
        var applicationAssembly = typeof(DependencyInjection).Assembly;
        var assemblies = new List<Assembly> { applicationAssembly };
        if (additionalAssemblies is { Length: > 0 })
        {
            assemblies.AddRange(additionalAssemblies);
        }

        services.AddScoped<IDispatcher, Dispatcher>();

        RegisterCqrsHandlers(services, assemblies);

        TypeAdapterConfig.GlobalSettings.Scan(assemblies.ToArray());

        return services;
    }

    private static void RegisterCqrsHandlers(IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var handlerInterfaces = new[]
        {
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>)
        };

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();

                foreach (var iface in interfaces)
                {
                    if (iface.IsGenericType && handlerInterfaces.Contains(iface.GetGenericTypeDefinition()))
                    {
                        services.AddScoped(iface, type);
                    }
                }
            }
        }
    }
}
