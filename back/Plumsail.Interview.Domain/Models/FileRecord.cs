using System.Text.Json.Serialization;
using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.Domain.Models;

public record FileRecord : ISubmission
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public long Size { get; init; }
    public string Type { get; init; } = string.Empty;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
    public string? PreSign { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SubmissionStatusEnum? Status { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedDate { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PriorityLevelEnum? Priority { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsPublic { get; init; }
    
    [JsonIgnore]
    public Stream? Stream { get; init; }
}