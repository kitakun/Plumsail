using System.Text.Json;

namespace Plumsail.Interview.Domain.Entities;

public class SubmissionEntity
{
    public Guid Id { get; set; }
    
    public FileData? FileData { get; set; }
    
    public JsonElement Payload { get; set; }
}