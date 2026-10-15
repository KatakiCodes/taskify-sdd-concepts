using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TaskFy.Api.Extensions;

public static class CorsExtensions
{
    public const string DefaultDevelopmentPolicyName = "DevelopmentCorsPolicy";

    public static IServiceCollection AddDevelopmentCors(
        this IServiceCollection services,
        string policyName = DefaultDevelopmentPolicyName)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName, policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseDevelopmentCors(
        this IApplicationBuilder app,
        string policyName = DefaultDevelopmentPolicyName)
    {
        app.UseCors(policyName);
        return app;
    }
}
