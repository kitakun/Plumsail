using Plumsail.Interview.Domain.Providers;
using Plumsail.Interview.Handlers;
using Plumsail.Interview.Handlers.FormReader;
using Plumsail.Interview.Handlers.SubmissionHandlers;
using Plumsail.Interview.MemoryFileStorage;

namespace Plumsail.Interview.Web.Extensions;

public static class PlumsailDependenciesRegistration
{
    public static void AddPlumsailDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IFileStorageProvider, MemoryFileStorageProvider>();
        services.AddSingleton<IIdentityProvider, GuidIdentityProvider>();
        services.AddSingleton<FileWithPropertiesFormReader<FileUploadData>, FileUploadFormReader>();

        services.AddHttpContextAccessor();
    }
}