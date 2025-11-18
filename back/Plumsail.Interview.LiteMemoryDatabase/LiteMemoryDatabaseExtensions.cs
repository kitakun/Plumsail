using Microsoft.Extensions.DependencyInjection;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.DatabaseContext.Services;
using Plumsail.Interview.LiteMemoryDatabase.Services;

namespace Plumsail.Interview.LiteMemoryDatabase;

public static class LiteMemoryDatabaseExtensions
{
    public static void AddLiteMemoryDatabase(this IServiceCollection services, string databaseName = "PlumsailLiteMemoryDb")
    {
        var connectionString = $"Data Source={databaseName}.db";
        services.AddSingleton<IDbConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
        services.AddScoped<ISubmissionDataService, SubmissionDataService>();
        services.AddSingleton<IDatabaseInitializer, LiteMemoryDatabaseInitializer>();
    }
}