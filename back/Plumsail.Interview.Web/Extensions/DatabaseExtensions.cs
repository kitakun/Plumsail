using Plumsail.Interview.DatabaseContext;

namespace Plumsail.Interview.Web.Extensions;

public static class DatabaseExtensions
{
    public static async Task EnsureDatabaseCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PlumsailDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}