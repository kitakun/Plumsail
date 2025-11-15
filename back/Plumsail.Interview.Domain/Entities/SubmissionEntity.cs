namespace Plumsail.Interview.Domain.Entities;

public class SubmissionEntity : ISubmission
{
    public Guid Id { get; set; }
    
    // File metadata
    public string Name { get; set; }
    public long Size { get; set; }
    public string Type { get; set; }
    
    // different typed properties
    public string? Description { get; set; }
    public SubmissionStatusEnum? Status { get; set; }
    public DateTime? CreatedDate { get; set; }
    public PriorityLevelEnum? Priority { get; set; }
    public bool? IsPublic { get; set; }
}