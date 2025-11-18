using System;
using System.IO;

using Dapper;
using Microsoft.Data.Sqlite;

using Plumsail.Interview.DatabaseContext;

namespace Plumsail.Interview.LiteMemoryDatabase;

[SqlSyntax(SqlSyntax.SQLite)]
public sealed class LiteMemoryDatabaseInitializer(IDbConnectionFactory connectionFactory) : IDatabaseInitializer
{
    private const string CreateTableSql = """
                                          CREATE TABLE IF NOT EXISTS "Submissions" (
                                              "Id" TEXT NOT NULL CONSTRAINT "PK_Submissions" PRIMARY KEY,
                                              "FileData" TEXT NULL,
                                              "Payload" TEXT NOT NULL
                                          );
                                          """;

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        try
        {
            var builder = new SqliteConnectionStringBuilder(connection.ConnectionString);
            DeleteDatabaseIfExists(builder);
        }
        finally
        {
            connection.Dispose();
        }

        await using var newConnection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = newConnection.CreateCommand();
        command.CommandText = CreateTableSql;
        if (command is SqliteCommand sqliteCommand)
        {
            await sqliteCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        else
        {
            command.ExecuteNonQuery();
        }
    }

    private static void DeleteDatabaseIfExists(SqliteConnectionStringBuilder builder)
    {
        var dataSource = builder.DataSource;
        if (string.IsNullOrWhiteSpace(dataSource))
        {
            return;
        }

        var databasePath = Path.IsPathRooted(dataSource)
            ? dataSource
            : Path.Combine(AppContext.BaseDirectory, dataSource);

        if (File.Exists(databasePath))
        {
            File.Delete(databasePath);
        }
    }
}