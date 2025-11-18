using System.Text.Json;
using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.LiteMemoryDatabase.Models;

namespace Plumsail.Interview.LiteMemoryDatabase.Extensions;

internal static class SubmissionRowExtensions
{
    internal static SubmissionEntity MapToEntity(this SubmissionRow row)
    {
        FileData? fileData = null;
        if (!string.IsNullOrWhiteSpace(row.FileData))
        {
            fileData = JsonSerializer.Deserialize<FileData>(row.FileData, DatabaseJsonSerializerContext.Default.FileData);
        }

        JsonElement payload;
        if (string.IsNullOrWhiteSpace(row.Payload))
        {
            using var emptyDocument = JsonDocument.Parse("{}");
            payload = emptyDocument.RootElement.Clone();
        }
        else
        {
            using var document = JsonDocument.Parse(row.Payload);
            payload = document.RootElement.Clone();
        }

        return new SubmissionEntity
        {
            Id = row.Id,
            FileData = fileData,
            Payload = payload
        };
    }
}

