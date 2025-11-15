using Mediator;

using Microsoft.EntityFrameworkCore;

using Plumsail.Interview.DatabaseContext;
using Plumsail.Interview.Domain.Entities;
using Plumsail.Interview.Domain.Models;

namespace Plumsail.Interview.Handlers.SubmissionHandlers;

public sealed record SubmissionsGetRequest(
    int? Offset = null,
    int? Limit = null
) : IRequest<OperationResult<Pagination<FileRecord>>>;

public sealed class SubmissionsGetHandler(
    PlumsailDbContext dbContext,
    IMediator mediator
) : IRequestHandler<SubmissionsGetRequest, OperationResult<Pagination<FileRecord>>>
{
    private static IQueryable<SubmissionEntity> GetSubmissionsQuery(PlumsailDbContext context)
    {
        return context.Submissions.AsNoTracking();
    }

    private static readonly Func<PlumsailDbContext, int, int, IAsyncEnumerable<FileRecord>> GetFileRecordsQuery =
        EF.CompileAsyncQuery(static (PlumsailDbContext context, int offset, int limit) =>
            context.Submissions
                .AsNoTracking()
                .OrderBy(static s => s.CreatedDate)
                .ThenBy(static s => s.Id)
                .Skip(offset)
                .Take(limit)
                .Select(static s => new FileRecord
                {
                    Id = s.Id,
                    Name = s.Name,
                    Size = s.Size,
                    Type = s.Type,
                    Description = s.Description,
                    Status = s.Status,
                    CreatedDate = s.CreatedDate,
                    Priority = s.Priority,
                    IsPublic = s.IsPublic
                })
        );

    public async ValueTask<OperationResult<Pagination<FileRecord>>> Handle(SubmissionsGetRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var totalCount = await GetSubmissionsQuery(dbContext).CountAsync(cancellationToken);

            var fileRecords = await GetFileRecordsQuery(
                    dbContext,
                    request.Offset ?? 0,
                    request.Limit ?? totalCount)
                .ToListAsync(cancellationToken);

            List<FileRecord> fileRecordsWithPreSign;

            if (fileRecords.Count == 0)
            {
                fileRecordsWithPreSign = fileRecords;
            }
            else
            {
                var preSignTasks = fileRecords.Select(async fileRecord =>
                {
                    try
                    {
                        var preSignRequest = new SubmissionGetPreSignRequest(fileRecord.Id);
                        var preSignResponse = await mediator.Send(preSignRequest, cancellationToken);
                        return new
                        {
                            FileRecord = fileRecord,
                            PreSignUrl = preSignResponse.IsSuccess
                                ? preSignResponse.Result
                                : null
                        };
                    }
                    catch
                    {
                        return new
                        {
                            FileRecord = fileRecord,
                            PreSignUrl = (string?) null
                        };
                    }
                });

                var preSignResults = await Task.WhenAll(preSignTasks);

                fileRecordsWithPreSign = preSignResults.Select(result =>
                    result.FileRecord with
                    {
                        PreSign = result.PreSignUrl
                    }
                ).ToList();
            }

            var result = new Pagination<FileRecord>
            {
                Items = fileRecordsWithPreSign,
                TotalCount = totalCount
            };

            return OperationResult<Pagination<FileRecord>>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<Pagination<FileRecord>>.Fail(ex);
        }
    }
}