namespace Plumsail.Interview.DatabaseContext;

public interface IDatabaseInitializer
{
    Task EnsureCreatedAsync(CancellationToken cancellationToken = default);
}

