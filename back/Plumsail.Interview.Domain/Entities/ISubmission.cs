using System.Text.Json;

namespace Plumsail.Interview.Domain.Entities;

public interface ISubmission
{
    Guid Id { get; }
    FileData FileData { get; }
    JsonElement Payload { get; }
}

