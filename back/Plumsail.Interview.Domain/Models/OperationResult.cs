using System.Text.Json.Serialization;

namespace Plumsail.Interview.Domain.Models;

public record OperationResult<T>
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Result { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Exception? Exception { get; init; }

    public bool IsSuccess => Exception == null;

    public static OperationResult<T> Success(T result) => new() { Result = result };
    public static OperationResult<T> Fail(Exception exception) => new() { Exception = exception };
}