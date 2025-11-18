using System.Data.Common;

using Microsoft.Data.Sqlite;

using Plumsail.Interview.DatabaseContext;

namespace Plumsail.Interview.LiteMemoryDatabase;

public sealed class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public async ValueTask<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}