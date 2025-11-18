using System.Text.Json;
using System.Text.Json.Serialization;

namespace Plumsail.Interview.Handlers;

/// <summary>
/// Source-generated JSON serializer context for Handlers (Dictionary serialization)
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false)]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
[JsonSerializable(typeof(List<JsonElement>))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(DateTime))]
[JsonSerializable(typeof(DateTime?))]
public partial class HandlersJsonSerializerContext : JsonSerializerContext
{
}