namespace Plumsail.Interview.LiteMemoryDatabase.Models;

internal record struct SubmissionRow(
    Guid Id,
    string? FileData,
    string Payload
);

