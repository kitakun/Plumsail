using Plumsail.Interview.Domain.Entities;

using System.Text.Json.Serialization;

namespace Plumsail.Interview.DatabaseContext;

/// <summary>
/// Source-generated JSON serializer context for DatabaseContext (EF Core value conversions)
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(FileData))]
[JsonSerializable(typeof(FileData?))]
public partial class DatabaseJsonSerializerContext : JsonSerializerContext
{
}

