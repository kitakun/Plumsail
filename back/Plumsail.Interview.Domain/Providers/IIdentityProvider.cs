namespace Plumsail.Interview.Domain.Providers;

/// <summary>
/// Entity ID generator
/// </summary>
public interface IIdentityProvider
{
    Guid GenerateId();
}