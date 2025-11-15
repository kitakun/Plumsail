using Plumsail.Interview.Domain.Providers;

namespace Plumsail.Interview.Handlers;

public sealed class GuidIdentityProvider : IIdentityProvider
{
    public Guid GenerateId() => Guid.NewGuid();
}