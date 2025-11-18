using Scalar.AspNetCore;

namespace Plumsail.Interview.Web.Extensions;

public static class ScalarServiceExtensions
{
    public static void AddScalar(this IServiceCollection services)
    {
        services.AddOpenApi();
    }

    public static void UseScalar(this WebApplication app)
    {
        app.MapOpenApi();

        app.MapScalarApiReference("/", options => { options.WithTitle("Plumsail Interview API"); });
    }
}