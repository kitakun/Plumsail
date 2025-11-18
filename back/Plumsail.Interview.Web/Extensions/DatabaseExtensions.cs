using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Plumsail.Interview.DatabaseContext;

namespace Plumsail.Interview.Web.Extensions;

public static class DatabaseExtensions
{
    public static async Task EnsureDatabaseCreatedAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.EnsureCreatedAsync(cancellationToken);
    }
}

