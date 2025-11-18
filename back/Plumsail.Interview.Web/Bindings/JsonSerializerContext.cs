using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plumsail.Interview.Web.Bindings;

/// <summary>
/// Source-generated JSON serializer context for AOT/trimmed builds
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false
)]
[JsonSerializable(typeof(int?))]

[JsonSerializable(typeof(OperationResult<Pagination<FileRecord>>))]
[JsonSerializable(typeof(OperationResult<FileRecord>))]
[JsonSerializable(typeof(OperationResult<IEnumerable<FileRecord>>))]
[JsonSerializable(typeof(OperationResult<List<FileRecord>>))]
[JsonSerializable(typeof(OperationResult<Dictionary<string, Guid>>))]
[JsonSerializable(typeof(OperationResult<Dictionary<string, JsonElement>>))]

[JsonSerializable(typeof(Pagination<FileRecord>))]

[JsonSerializable(typeof(FileRecord))]
[JsonSerializable(typeof(FileData))]

[JsonSerializable(typeof(IEnumerable<FileRecord>))]
[JsonSerializable(typeof(List<FileRecord>))]

[JsonSerializable(typeof(Dictionary<string, Guid>))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]

[JsonSerializable(typeof(PriorityLevelEnum))]
[JsonSerializable(typeof(SubmissionStatusEnum))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}