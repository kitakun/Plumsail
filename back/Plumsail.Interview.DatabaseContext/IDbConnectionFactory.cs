using System.Data.Common;

namespace Plumsail.Interview.DatabaseContext;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}

