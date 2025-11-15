using System.Text.Json;
using System.Text.Json.Serialization;
using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.Domain.Models;

public record FileRecord : ISubmission
{
    public Guid Id { get; init; }
    
    public FileData FileData { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
    public string? PreSign { get; init; }
    
    public JsonElement Payload { get; init; }
    
    [JsonIgnore]
    public Stream? Stream { get; init; }
}