namespace Plumsail.Interview.Domain.Models;

public class Pagination<T>
{
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
}