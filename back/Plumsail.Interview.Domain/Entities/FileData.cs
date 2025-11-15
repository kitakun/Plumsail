namespace Plumsail.Interview.Domain.Entities;

public readonly record struct FileData(
    string Name,
    long Size,
    string Type);

