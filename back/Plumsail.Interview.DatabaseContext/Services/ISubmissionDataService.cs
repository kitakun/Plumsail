using Plumsail.Interview.Domain.Entities;

namespace Plumsail.Interview.DatabaseContext.Services;

public interface ISubmissionDataService
{
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<SubmissionEntity>> GetPageAsync(int offset, int limit, CancellationToken cancellationToken);

    Task<IReadOnlyList<SubmissionEntity>> GetAllAsync(CancellationToken cancellationToken);

    Task<SubmissionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

    Task InsertAsync(IEnumerable<SubmissionEntity> submissions, CancellationToken cancellationToken);
}

