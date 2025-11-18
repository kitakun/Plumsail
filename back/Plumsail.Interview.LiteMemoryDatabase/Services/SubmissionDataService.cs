using Dapper;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.DatabaseContext.Services;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.LiteMemoryDatabase.Extensions;
using Plumsail.Interview.LiteMemoryDatabase.Models;

using System.Text.Json;

namespace Plumsail.Interview.LiteMemoryDatabase.Services;

[SqlSyntax(SqlSyntax.SQLite)]
public class SubmissionDataService(IDbConnectionFactory connectionFactory)
    : ISubmissionDataService
{
    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
                               SELECT COUNT(*)
                               FROM "Submissions"
                           """;

        return await conn.QueryFirstAsync<int>(sql);
    }

    public async Task<IReadOnlyList<SubmissionEntity>> GetPageAsync(
        int offset,
        int limit,
        CancellationToken cancellationToken)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
                               SELECT "Id", "FileData", "Payload"
                               FROM "Submissions"
                               ORDER BY "Id"
                               LIMIT @limit OFFSET @offset
                           """;

        var rows = await conn.QueryAsync<SubmissionRow>(sql, new { offset, limit });
        return rows.Select(row => row.MapToEntity()).ToList();
    }

    public async Task<IReadOnlyList<SubmissionEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
                               SELECT "Id", "FileData", "Payload"
                               FROM "Submissions"
                               ORDER BY "Id"
                           """;

        var rows = await conn.QueryAsync<SubmissionRow>(sql);
        return rows.Select(row => row.MapToEntity()).ToList();
    }

    public async Task<SubmissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
                               SELECT "Id", "FileData", "Payload"
                               FROM "Submissions"
                               WHERE "Id" = @id
                               LIMIT 1
                           """;

        var row = await conn.QueryFirstOrDefaultAsync<SubmissionRow>(sql, new { id });
        return row.Id != Guid.Empty ? row.MapToEntity() : null;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
                               SELECT 1
                               FROM "Submissions"
                               WHERE "Id" = @id
                               LIMIT 1
                           """;

        var result = await conn.ExecuteScalarAsync<int?>(sql, new { id });

        return result.HasValue;
    }

    public async Task InsertAsync(IEnumerable<SubmissionEntity> submissions, CancellationToken cancellationToken)
    {
        var list = submissions.ToList();
        if (list.Count == 0) return;

        await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = conn.BeginTransaction();

        const string sql = """
                               INSERT INTO "Submissions" ("Id", "FileData", "Payload")
                               VALUES (@Id, @FileData, @Payload)
                           """;

        var rows = list.Select(entity => new SubmissionRow(
            entity.Id,
            entity.FileData.HasValue
                ? JsonSerializer.Serialize(entity.FileData.Value, DatabaseJsonSerializerContext.Default.FileData)
                : null,
            entity.Payload.GetRawText()
        )).ToList();

        await conn.ExecuteAsync(sql, rows, transaction: transaction);

        transaction.Commit();
    }
}