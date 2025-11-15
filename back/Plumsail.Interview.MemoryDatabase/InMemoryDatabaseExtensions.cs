using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Plumsail.Interview.DatabaseContext;

namespace Plumsail.Interview.MemoryDatabase;

public static class InMemoryDatabaseExtensions
{
    public static void AddInMemoryDatabase(this IServiceCollection services, string databaseName = "PlumsailInMemoryDb")
    {
        services.AddDbContext<PlumsailDbContext>(options =>
            options.UseInMemoryDatabase(databaseName));
    }
}