namespace Plumsail.Interview.Domain.Entities;

public interface ISubmission
{
    Guid Id { get; }
    string Name { get; }
    long Size { get; }
    string Type { get; }
    string? Description { get; }
    SubmissionStatusEnum? Status { get; }
    DateTime? CreatedDate { get; }
    PriorityLevelEnum? Priority { get; }
    bool? IsPublic { get; }
}

